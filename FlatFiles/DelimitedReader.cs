using System;
using System.IO;
using System.Threading.Tasks;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <inheritdoc />
    /// <summary>
    /// Extracts records from a file containing delimited values.
    /// </summary>
    public sealed class DelimitedReader : IReader, IReaderWithMetadata
    {
        private readonly DelimitedRecordParser parser;
        private readonly DelimitedSchemaSelector? schemaSelector;
        private DelimitedSchema? schema;
        private IRecordContext? recordContext;
        private int physicalRecordNumber;
        private int logicalRecordNumber;
        private object?[]? values;
        private bool endOfFile;
        private bool hasError;

        /// <summary>
        /// Initializes a new DelimitedReader with no schema.
        /// </summary>
        /// <param name="reader">A reader over the delimited document.</param>
        /// <param name="options">The options controlling how the delimited document is read.</param>
        /// <exception cref="ArgumentNullException">The reader is null.</exception>
        public DelimitedReader(TextReader reader, DelimitedOptions? options = null)
            : this(reader, null, options, false)
        {
        }

        /// <summary>
        /// Initializes a new DelimitedReader with the given schema.
        /// </summary>
        /// <param name="reader">A reader over the delimited document.</param>
        /// <param name="schema">The schema of the delimited document.</param>
        /// <param name="options">The options controlling how the delimited document is read.</param>
        /// <exception cref="ArgumentNullException">The reader is null.</exception>
        /// <exception cref="ArgumentNullException">The schema is null.</exception>
        public DelimitedReader(TextReader reader, DelimitedSchema schema, DelimitedOptions? options = null)
            : this(reader, schema, options, true)
        {
        }

        /// <summary>
        /// Initializes a new DelimitedReader with the given schema.
        /// </summary>
        /// <param name="reader">A reader over the delimited document.</param>
        /// <param name="schemaSelector">The schema selector configured to determine the schema dynamically.</param>
        /// <param name="options">The options controlling how the delimited document is read.</param>
        /// <exception cref="ArgumentNullException">The reader is null.</exception>
        /// <exception cref="ArgumentNullException">The schema selector is null.</exception>
        public DelimitedReader(TextReader reader, DelimitedSchemaSelector schemaSelector, DelimitedOptions? options = null)
            : this(reader, null, options, false)
        {
            this.schemaSelector = schemaSelector ?? throw new ArgumentNullException(nameof(schemaSelector));
        }

        private DelimitedReader(TextReader reader, DelimitedSchema? schema, DelimitedOptions? options, bool hasSchema)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            if (hasSchema && schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }
            options = options == null ? new DelimitedOptions() : options.Clone();
            if (options.RecordSeparator == options.Separator)
            {
                throw new ArgumentException(Resources.SameSeparator, nameof(options));
            }
            var retryReader = new RetryReader(reader);
            this.parser = new DelimitedRecordParser(retryReader, options);
            this.schema = schema;
        }

        /// <summary>
        /// Raised when a record is read but before its columns are parsed.
        /// </summary>
        public event EventHandler<DelimitedRecordReadEventArgs>? RecordRead;

        /// <summary>
        /// Raised when a record is parsed.
        /// </summary>
        public event EventHandler<DelimitedRecordParsedEventArgs>? RecordParsed;

        event EventHandler<IRecordParsedEventArgs>? IReader.RecordParsed
        {
            add
            {
                if (value != null)
                {
                    RecordParsed += (sender, e) => value(sender, e);
                }
            }
            remove
            {
                if (value != null)
                {
                    RecordParsed -= (sender, e) => value(sender, e);
                }
            }
        }

        /// <summary>
        /// Raised when an error occurs while processing a record.
        /// </summary>
        public event EventHandler<RecordErrorEventArgs>? RecordError;

        /// <summary>
        /// Raised when an error occurs while processing a column.
        /// </summary>
        public event EventHandler<ColumnErrorEventArgs>? ColumnError;

        IOptions IReader.Options => parser.Options;

        /// <summary>
        /// Gets the schema being used by the parser. If a 
        /// SchemaSelector was provided, null will be returned.
        /// If no schema was specified and no schema exists in
        /// the file, null will be returned.
        /// </summary>
        /// <returns>The names.</returns>
        public DelimitedSchema? GetSchema()
        {
            if (schemaSelector != null)
            {
                return null;
            }
            return HandleSchema();
        }

        /// <summary>
        /// Gets the schema being used by the parser.If a 
        /// SchemaSelector was provided, null will be returned.
        /// If no schema was specified and no schema exists in
        /// the file, null will be returned.
        /// </summary>
        /// <returns>The schema being used by the parser.</returns>
        public async Task<DelimitedSchema?> GetSchemaAsync()
        {
            if (schemaSelector != null)
            {
                return null;
            }
            return await HandleSchemaAsync();
        }

        ISchema? IReader.GetSchema()
        {
            return GetSchema();
        }

        async Task<ISchema?> IReader.GetSchemaAsync()
        {
            var schema = await GetSchemaAsync().ConfigureAwait(false);
            return schema;
        }

        /// <summary>
        /// Attempts to read the next record from the stream.
        /// </summary>
        /// <returns>True if the next record was read or false if all records have been read.</returns>
        public bool Read()
        {
            if (hasError)
            {
                throw new InvalidOperationException(Resources.ReadingWithErrors);
            }
            HandleSchema();
            try
            {
                values = ParsePartitions();
                if (values == null)
                {
                    return false;
                }

                ++logicalRecordNumber;
                return true;
            }
            catch (FlatFileException)
            {
                hasError = true;
                throw;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Attempts to read the next record from the stream.
        /// </summary>
        /// <returns>True if the next record was read or false if all records have been read.</returns>
        public async ValueTask<bool> ReadAsync()
        {
            if (hasError)
            {
                throw new InvalidOperationException(Resources.ReadingWithErrors);
            }
            await HandleSchemaAsync().ConfigureAwait(false);
            try
            {
                values = await ParsePartitionsAsync().ConfigureAwait(false);
                if (values == null)
                {
                    return false;
                }

                ++logicalRecordNumber;
                return true;
            }
            catch (FlatFileException)
            {
                hasError = true;
                throw;
            }
        }

        private DelimitedSchema? HandleSchema()
        {
            if (physicalRecordNumber != 0)
            {
                return this.schema;
            }
            if (!parser.Options.IsFirstRecordSchema)
            {
                return this.schema;
            }
            if (schemaSelector != null || this.schema != null)
            {
                SkipInternal();
                return this.schema;
            }
            var (_, columnNames) = ReadNextRecord();
            if (columnNames == null)
            {
                // Do not treat a missing schema in an empty file as an error.
                return null;
            }
            this.schema = CreateSchemaFromHeader(columnNames);
            return this.schema;
        }

        private async Task<DelimitedSchema?> HandleSchemaAsync()
        {
            if (physicalRecordNumber != 0)
            {
                return this.schema;
            }
            if (!parser.Options.IsFirstRecordSchema)
            {
                return this.schema;
            }
            if (schemaSelector != null || this.schema != null)
            {
                await SkipAsyncInternal().ConfigureAwait(false);
                return this.schema;
            }
            var (_, columnNames) = await ReadNextRecordAsync().ConfigureAwait(false);
            if (columnNames == null)
            {
                // Do not treat a missing schema in an empty file as an error.
                return null;
            }
            this.schema = CreateSchemaFromHeader(columnNames);
            return this.schema;
        }

        private DelimitedSchema CreateSchemaFromHeader(string[] columnNames)
        {
            var schema = new DelimitedSchema();
            foreach (string columnName in columnNames)
            {
                var column = new StringColumn(columnName)
                {
                    Trim = !parser.Options.PreserveWhiteSpace
                };
                schema.AddColumn(column);
            }
            return schema;
        }

        private object?[]? ParsePartitions()
        {
            while (!endOfFile)
            {
                var (record, rawValues) = ReadNextRecord();
                var values = ProcessRecord(record, rawValues);
                if (values != null)
                {
                    return values;
                }
            }
            return null;
        }

        private async Task<object?[]?> ParsePartitionsAsync()
        {
            while (!endOfFile)
            {
                var (record, rawValues) = await ReadNextRecordAsync();
                var values = ProcessRecord(record, rawValues);
                if (values != null)
                {
                    return values;
                }
            }
            return null;
        }

        private object?[]? ProcessRecord(string? record, string[]? rawValues)
        {
            if (record == null || rawValues == null)
            {
                return null;
            }
            var schema = GetSchema(record, rawValues);
            if (schema == null)
            {
                schema = DelimitedSchema.BuildDynamicSchema(parser.Options, rawValues.Length);
            }
            var recordContext = NewRecordContext(schema, record, rawValues);
            this.recordContext = recordContext;
            if (IsSkipped(recordContext, rawValues))
            {
                return null;
            }
            if (HasWrongNumberOfColumns(schema, rawValues))
            {
                ProcessError(new RecordProcessingException(recordContext, Resources.DelimitedRecordWrongNumberOfColumns));
                return null;
            }
            object?[]? values = ParseValues(recordContext, rawValues);
            if (values == null)
            {
                return null;
            }
            RecordParsed?.Invoke(this, new DelimitedRecordParsedEventArgs(recordContext, values));
            return values;
        }


        private DelimitedSchema? GetSchema(string? record, string[] rawValues)
        {
            if (schemaSelector == null)
            {
                return this.schema;
            }
            var schema = schemaSelector.GetSchema(rawValues);
            if (schema != null)
            {
                return schema;
            }
            var recordContext = GetMetadata(null, record);
            ProcessError(new RecordProcessingException(recordContext, Resources.MissingMatcher));
            return null;
        }

        private bool IsSkipped(DelimitedRecordContext recordContext, string[] values)
        {
            if (RecordRead == null)
            {
                return false;
            }
            var e = new DelimitedRecordReadEventArgs(recordContext, values);
            RecordRead(this, e);
            return e.IsSkipped;
        }

        private DelimitedRecordContext NewRecordContext(DelimitedSchema schema, string record, string[] values)
        {
            var executionContext = new DelimitedExecutionContext(schema, parser.Options.Clone());
            var recordContext = new DelimitedRecordContext(executionContext)
            {
                PhysicalRecordNumber = physicalRecordNumber,
                LogicalRecordNumber = logicalRecordNumber,
                Record = record,
                Values = values
            };
            return recordContext;
        }

        private bool HasWrongNumberOfColumns(DelimitedSchema schema, string[] values)
        {
            var columnDefinitions = schema.ColumnDefinitions;
            return values.Length + columnDefinitions.MetadataCount < columnDefinitions.PhysicalCount;
        }

        private object?[]? ParseValues(DelimitedRecordContext recordContext, string[] rawValues)
        {
            try
            {
                recordContext.ColumnError += ColumnError;
                var schema = recordContext.ExecutionContext.Schema;
                return schema.ParseValues(recordContext, rawValues);
            }
            catch (FlatFileException exception)
            {
                ProcessError(new RecordProcessingException(recordContext, Resources.InvalidRecordConversion, exception));
                return null;
            }
        }

        /// <summary>
        /// Attempts to skip the next record from the stream.
        /// </summary>
        /// <returns>True if the next record was skipped or false if all records have been read.</returns>
        /// <remarks>The previously parsed values remain available.</remarks>
        public bool Skip()
        {
            if (hasError)
            {
                throw new InvalidOperationException(Resources.ReadingWithErrors);
            }
            HandleSchema();
            bool result = SkipInternal();
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Attempts to skip the next record from the stream.
        /// </summary>
        /// <returns>True if the next record was skipped or false if all records have been read.</returns>
        /// <remarks>The previously parsed values remain available.</remarks>
        public async ValueTask<bool> SkipAsync()
        {
            if (hasError)
            {
                throw new InvalidOperationException(Resources.ReadingWithErrors);
            }
            await HandleSchemaAsync().ConfigureAwait(false);
            var result = await SkipAsyncInternal().ConfigureAwait(false);
            return result;
        }

        private bool SkipInternal()
        {
            var (_, rawValues) = ReadNextRecord();
            return rawValues != null;
        }

        private async ValueTask<bool> SkipAsyncInternal()
        {
            var (_, rawValues) = await ReadNextRecordAsync().ConfigureAwait(false);
            return rawValues != null;
        }

        private void ProcessError(RecordProcessingException exception)
        {
            if (RecordError != null)
            {
                var args = new RecordErrorEventArgs(exception);
                RecordError(this, args);
                if (args.IsHandled)
                {
                    return;
                }
            }
            throw exception;
        }

        private (string?, string[]?) ReadNextRecord()
        {
            if (parser.IsEndOfStream())
            {
                endOfFile = true;
                values = null;
                return (null, null);
            }
            try
            {
                var (record, results) = parser.ReadRecord();
                ++physicalRecordNumber;
                return (record, results);
            }
            catch (DelimitedSyntaxException exception)
            {
                // If we cannot read the next record, we cannot process it or allow it to be ignored.
                // We must treat it as a fatal error.
                var recordContext = GetMetadata(null, null);
                throw new RecordProcessingException(recordContext, Resources.InvalidRecordFormatNumber, exception);
            }
        }

        private async Task<(string?, string[]?)> ReadNextRecordAsync()
        {
            if (await parser.IsEndOfStreamAsync().ConfigureAwait(false))
            {
                endOfFile = true;
                values = null;
                return (null, null);
            }
            try
            {
                var (record, results) = await parser.ReadRecordAsync().ConfigureAwait(false);
                ++physicalRecordNumber;
                return (record, results);
            }
            catch (DelimitedSyntaxException exception)
            {
                var recordContext = GetMetadata(null, null);
                throw new RecordProcessingException(recordContext, Resources.InvalidRecordFormatNumber, exception);
            }
        }

        /// <summary>
        /// Gets the values for the current record.
        /// </summary>
        /// <returns>The values of the current record.</returns>
        public object?[] GetValues()
        {
            if (hasError)
            {
                throw new InvalidOperationException(Resources.ReadingWithErrors);
            }
            if (physicalRecordNumber == 0)
            {
                throw new InvalidOperationException(Resources.ReadNotCalled);
            }
            if (endOfFile || values == null)
            {
                throw new InvalidOperationException(Resources.NoMoreRecords);
            }
            object[] copy = new object[values.Length];
            Array.Copy(values, copy, values.Length);
            return copy;
        }

        private IRecordContext GetMetadata(DelimitedSchema? schema, string? record)
        {
            if (this.recordContext != null)
            {
                return this.recordContext;
            }
            var executionContext = new GenericExecutionContext(schema, parser.Options.Clone());
            var recordContext = new GenericRecordContext(executionContext)
            {
                PhysicalRecordNumber = physicalRecordNumber,
                LogicalRecordNumber = logicalRecordNumber,
                Record = record
            };
            return recordContext;
        }

        IRecordContext IReaderWithMetadata.GetMetadata()
        {
            return GetMetadata(null, null);
        }

        internal void SetSchema(DelimitedSchema schema)
        {
            this.schema = schema;
        }
    }
}
