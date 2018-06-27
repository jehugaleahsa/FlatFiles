using System;
using System.IO;
using System.Threading.Tasks;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <inheritdoc />
    /// <summary>
    /// Extracts records from a file that has values separated by a separator token.
    /// </summary>
    public sealed class SeparatedValueReader : IReader, IReaderWithMetadata
    {
        private readonly SeparatedValueRecordParser parser;
        private readonly SeparatedValueSchemaSelector schemaSelector;
        private readonly ProcessMetadata metadata;
        private object[] values;
        private bool endOfFile;
        private bool hasError;

        /// <summary>
        /// Initializes a new SeparatedValueReader with no schema.
        /// </summary>
        /// <param name="reader">A reader over the separated value document.</param>
        /// <param name="options">The options controlling how the separated value document is read.</param>
        /// <exception cref="ArgumentNullException">The reader is null.</exception>
        public SeparatedValueReader(TextReader reader, SeparatedValueOptions options = null)
            : this(reader, null, options, false)
        {
        }

        /// <summary>
        /// Initializes a new SeparatedValueReader with the given schema.
        /// </summary>
        /// <param name="reader">A reader over the separated value document.</param>
        /// <param name="schema">The schema of the separated value document.</param>
        /// <param name="options">The options controlling how the separated value document is read.</param>
        /// <exception cref="ArgumentNullException">The reader is null.</exception>
        /// <exception cref="ArgumentNullException">The schema is null.</exception>
        public SeparatedValueReader(TextReader reader, SeparatedValueSchema schema, SeparatedValueOptions options = null)
            : this(reader, schema, options, true)
        {
        }

        /// <summary>
        /// Initializes a new SeparatedValueReader with the given schema.
        /// </summary>
        /// <param name="reader">A reader over the separated value document.</param>
        /// <param name="schemaSelector">The schema selector configured to determine the schema dynamically.</param>
        /// <param name="options">The options controlling how the separated value document is read.</param>
        /// <exception cref="ArgumentNullException">The reader is null.</exception>
        /// <exception cref="ArgumentNullException">The schema selector is null.</exception>
        public SeparatedValueReader(TextReader reader, SeparatedValueSchemaSelector schemaSelector, SeparatedValueOptions options = null)
            : this(reader, null, options, false)
        {
            this.schemaSelector = schemaSelector ?? throw new ArgumentNullException(nameof(schemaSelector));
        }

        private SeparatedValueReader(TextReader reader, SeparatedValueSchema schema, SeparatedValueOptions options, bool hasSchema)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            if (hasSchema && schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }
            if (options == null)
            {
                options = new SeparatedValueOptions();
            }
            if (options.RecordSeparator == options.Separator)
            {
                throw new ArgumentException(Resources.SameSeparator, nameof(options));
            }
            RetryReader retryReader = new RetryReader(reader);
            parser = new SeparatedValueRecordParser(retryReader, options);
            metadata = new ProcessMetadata()
            {
                Schema = hasSchema ? schema : null,
                Options = parser.Options
            };
        }

        /// <summary>
        /// Raised when a record is read but before its columns are parsed.
        /// </summary>
        public event EventHandler<SeparatedValueRecordReadEventArgs> RecordRead;

        /// <summary>
        /// Raised when a record is parsed.
        /// </summary>
        public event EventHandler<SeparatedValueRecordParsedEventArgs> RecordParsed;

        event EventHandler<IRecordParsedEventArgs> IReader.RecordParsed
        {
            add => RecordParsed += (sender, e) => value(sender, e);
            remove => RecordParsed -= (sender, e) => value(sender, e);
        }

        /// <summary>
        /// Raised when an error occurs while processing a record.
        /// </summary>
        public event EventHandler<ProcessingErrorEventArgs> Error;

        /// <summary>
        /// Gets the names of the columns found in the file.
        /// </summary>
        /// <returns>The names.</returns>
        public SeparatedValueSchema GetSchema()
        {
            if (schemaSelector != null)
            {
                return null;
            }
            HandleSchema();
            if (metadata.Schema == null)
            {
                throw new InvalidOperationException(Resources.SchemaNotDefined);
            }
            return metadata.Schema;
        }

        ISchema IReader.GetSchema()
        {
            return GetSchema();
        }

        /// <summary>
        /// Gets the schema being used by the parser.
        /// </summary>
        /// <returns>The schema being used by the parser.</returns>
        public async Task<SeparatedValueSchema> GetSchemaAsync()
        {
            if (schemaSelector != null)
            {
                return null;
            }
            await HandleSchemaAsync().ConfigureAwait(false);
            if (metadata.Schema == null)
            {
                throw new InvalidOperationException(Resources.SchemaNotDefined);
            }
            return metadata.Schema;
        }

        async Task<ISchema> IReader.GetSchemaAsync()
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

                ++metadata.LogicalRecordCount;
                return true;
            }
            catch (FlatFileException)
            {
                hasError = true;
                throw;
            }
        }

        private void HandleSchema()
        {
            if (metadata.RecordCount != 0)
            {
                return;
            }
            if (!parser.Options.IsFirstRecordSchema)
            {
                return;
            }
            if (schemaSelector != null || metadata.Schema != null)
            {
                skip();
                return;
            }
            string[] columnNames = ReadNextRecord();
            metadata.Schema = new SeparatedValueSchema();
            foreach (string columnName in columnNames)
            {
                StringColumn column = new StringColumn(columnName);
                metadata.Schema.AddColumn(column);
            }
        }

        private object[] ParsePartitions()
        {
            var rawValues = ReadWithFilter();
            while (rawValues != null)
            {
                if (metadata.Schema != null && hasWrongNumberOfColumns(rawValues))
                {
                    ProcessError(new RecordProcessingException(metadata.RecordCount, Resources.SeparatedValueRecordWrongNumberOfColumns));
                }
                else
                {
                    object[] values = ParseValues(rawValues);
                    if (values != null)
                    {
                        RecordParsed?.Invoke(this, new SeparatedValueRecordParsedEventArgs(metadata, values));
                        return values;
                    }
                }
                rawValues = ReadWithFilter();
            }
            return null;
        }

        private string[] ReadWithFilter()
        {
            string[] rawValues = ReadNextRecord();
            metadata.Schema = GetSchema(rawValues);
            while (rawValues != null && IsSkipped(rawValues))
            {
                rawValues = ReadNextRecord();
                metadata.Schema = GetSchema(rawValues);
            }
            return rawValues;
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

                ++metadata.LogicalRecordCount;
                return true;
            }
            catch (FlatFileException)
            {
                hasError = true;
                throw;
            }
        }

        private async Task HandleSchemaAsync()
        {
            if (metadata.RecordCount != 0)
            {
                return;
            }
            if (!parser.Options.IsFirstRecordSchema)
            {
                return;
            }
            if (metadata.Schema != null)
            {
                await skipAsync().ConfigureAwait(false);
                return;
            }
            string[] columnNames = await ReadNextRecordAsync().ConfigureAwait(false);
            metadata.Schema = new SeparatedValueSchema();
            foreach (string columnName in columnNames)
            {
                StringColumn column = new StringColumn(columnName);
                metadata.Schema.AddColumn(column);
            }
        }

        private async Task<object[]> ParsePartitionsAsync()
        {
            var rawValues = await ReadWithFilterAsync().ConfigureAwait(false);
            while (rawValues != null)
            {
                if (metadata.Schema != null && hasWrongNumberOfColumns(rawValues))
                {
                    ProcessError(new RecordProcessingException(metadata.RecordCount, Resources.SeparatedValueRecordWrongNumberOfColumns));
                }
                else
                {
                    object[] values = ParseValues(rawValues);
                    if (values != null)
                    {
                        return values;
                    }
                }
                rawValues = await ReadWithFilterAsync().ConfigureAwait(false);
            }
            return null;
        }

        private bool hasWrongNumberOfColumns(string[] values)
        {
            var schema = metadata.Schema;
            return values.Length + schema.ColumnDefinitions.MetadataCount < schema.ColumnDefinitions.PhysicalCount;
        }

        private async Task<string[]> ReadWithFilterAsync()
        {
            string[] rawValues = await ReadNextRecordAsync().ConfigureAwait(false);
            metadata.Schema = GetSchema(rawValues);
            while (rawValues != null && IsSkipped(rawValues))
            {
                rawValues = await ReadNextRecordAsync().ConfigureAwait(false);
                metadata.Schema = GetSchema(rawValues);
            }
            return rawValues;
        }

        private SeparatedValueSchema GetSchema(string[] rawValues)
        {
            if (rawValues == null)
            {
                return null;
            }
            if (schemaSelector == null)
            {
                return metadata.Schema;
            }
            return schemaSelector.GetSchema(rawValues);
        }

        private bool IsSkipped(string[] values)
        {
            if (RecordRead == null)
            {
                return false;
            }
            var e = new SeparatedValueRecordReadEventArgs(metadata, values);
            RecordRead(this, e);
            return e.IsSkipped;
        }

        private object[] ParseValues(string[] rawValues)
        {
            if (metadata.Schema == null)
            {
                return rawValues;
            }
            try
            {
                return metadata.Schema.ParseValues(metadata, rawValues);
            }
            catch (FlatFileException exception)
            {
                ProcessError(new RecordProcessingException(metadata.RecordCount, Resources.InvalidRecordConversion, exception));
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
            bool result = skip();
            return result;
        }

        private bool skip()
        {
            string[] rawValues = ReadNextRecord();
            return rawValues != null;
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
            bool result = await skipAsync().ConfigureAwait(false);
            return result;
        }

        private async ValueTask<bool> skipAsync()
        {
            string[] rawValues = await ReadNextRecordAsync().ConfigureAwait(false);
            return rawValues != null;
        }

        private void ProcessError(RecordProcessingException exception)
        {
            if (Error != null)
            {
                var args = new ProcessingErrorEventArgs(exception);
                Error(this, args);
                if (args.IsHandled)
                {
                    return;
                }
            }
            throw exception;
        }

        private string[] ReadNextRecord()
        {
            if (parser.IsEndOfStream())
            {
                endOfFile = true;
                values = null;
                return null;
            }
            try
            {
                string[] results = parser.ReadRecord();
                ++metadata.RecordCount;
                return results;
            }
            catch (SeparatedValueSyntaxException exception)
            {
                throw new RecordProcessingException(metadata.RecordCount, Resources.InvalidRecordFormatNumber, exception);
            }
        }

        private async Task<string[]> ReadNextRecordAsync()
        {
            if (await parser.IsEndOfStreamAsync().ConfigureAwait(false))
            {
                endOfFile = true;
                values = null;
                return null;
            }
            try
            {
                string[] results = await parser.ReadRecordAsync().ConfigureAwait(false);
                ++metadata.RecordCount;
                return results;
            }
            catch (SeparatedValueSyntaxException exception)
            {
                throw new RecordProcessingException(metadata.RecordCount, Resources.InvalidRecordFormatNumber, exception);
            }
        }

        /// <summary>
        /// Gets the values for the current record.
        /// </summary>
        /// <returns>The values of the current record.</returns>
        public object[] GetValues()
        {
            if (hasError)
            {
                throw new InvalidOperationException(Resources.ReadingWithErrors);
            }
            if (metadata.RecordCount == 0)
            {
                throw new InvalidOperationException(Resources.ReadNotCalled);
            }
            if (endOfFile)
            {
                throw new InvalidOperationException(Resources.NoMoreRecords);
            }
            object[] copy = new object[values.Length];
            Array.Copy(values, copy, values.Length);
            return copy;
        }

        IProcessMetadata IReaderWithMetadata.GetMetadata()
        {
            return metadata;
        }

        private class ProcessMetadata : IProcessMetadata
        {
            public SeparatedValueSchema Schema { get; set; }

            ISchema IProcessMetadata.Schema => Schema;

            public SeparatedValueOptions Options { get; set; }

            IOptions IProcessMetadata.Options => Options;

            public int RecordCount { get; set; }

            public int LogicalRecordCount { get; set; }
        }
    }
}
