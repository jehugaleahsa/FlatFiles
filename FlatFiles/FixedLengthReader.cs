using System;
using System.IO;
using System.Threading.Tasks;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Extracts records from a file that has value in fixed-length columns.
    /// </summary>
    public sealed class FixedLengthReader : IReader, IReaderWithMetadata
    {
        private readonly FixedLengthRecordParser parser;
        private readonly FixedLengthSchemaSelector schemaSelector;
        private readonly FixedLengthRecordContext metadata;
        private object[] values;
        private bool endOfFile;
        private bool hasError;

        /// <summary>
        /// Initializes a new FixedLengthReader with the given schema.
        /// </summary>
        /// <param name="reader">A reader over the fixed-length document.</param>
        /// <param name="schema">The schema of the fixed-length document.</param>
        /// <param name="options">The options controlling how the fixed-length document is read.</param>
        /// <exception cref="ArgumentNullException">The reader is null.</exception>
        /// <exception cref="ArgumentNullException">The schema is null.</exception>
        public FixedLengthReader(TextReader reader, FixedLengthSchema schema, FixedLengthOptions options = null)
            : this(reader, schema, options, true)
        {
        }

        /// <summary>
        /// Initializes a new FixedLengthReader with the given schema.
        /// </summary>
        /// <param name="reader">A reader over the fixed-length document.</param>
        /// <param name="schemaSelector">The schema selector configured to determine the schema dynamically.</param>
        /// <param name="options">The options controlling how the fixed-length document is read.</param>
        /// <exception cref="ArgumentNullException">The reader is null.</exception>
        /// <exception cref="ArgumentNullException">The schema selector is null.</exception>
        public FixedLengthReader(TextReader reader, FixedLengthSchemaSelector schemaSelector, FixedLengthOptions options = null)
            : this(reader, null, options, false)
        {
            this.schemaSelector = schemaSelector ?? throw new ArgumentNullException(nameof(schemaSelector));
        }

        private FixedLengthReader(TextReader reader, FixedLengthSchema schema, FixedLengthOptions options = null, bool hasSchema = true)
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
                options = new FixedLengthOptions();
            }
            parser = new FixedLengthRecordParser(reader, schema, options);
            metadata = new FixedLengthRecordContext()
            {
                ExecutionContext = new FixedLengthExecutionContext()
                {
                    Schema = schema,
                    Options = options.Clone()
                }
            };
        }

        /// <summary>
        /// Raised when a record is read from the source file, before it is partitioned.
        /// </summary>
        public event EventHandler<FixedLengthRecordReadEventArgs> RecordRead;

        /// <summary>
        /// Raised after a record is partitioned, before it is parsed.
        /// </summary>
        public event EventHandler<FixedLengthRecordPartitionedEventArgs> RecordPartitioned;

        /// <summary>
        /// Raised after a record is parsed.
        /// </summary>
        public event EventHandler<FixedLengthRecordParsedEventArgs> RecordParsed;

        event EventHandler<IRecordParsedEventArgs> IReader.RecordParsed
        {
            add => RecordParsed += (sender, e) => value(sender, e);
            remove => RecordParsed -= (sender, e) => value(sender, e);
        }

        /// <summary>
        /// Raised when an error occurs while processing a record.
        /// </summary>
        public event EventHandler<RecordErrorEventArgs> RecordError;

        /// <summary>
        /// Raised when an error occurs while processing a column.
        /// </summary>
        public event EventHandler<ColumnErrorEventArgs> ColumnError
        {
            add => metadata.ColumnError += value;
            remove => metadata.ColumnError -= value;
        }

        /// <summary>
        /// Gets the schema being used by the parser.
        /// </summary>
        /// <returns>The schema being used by the parser.</returns>
        public FixedLengthSchema GetSchema()
        {
            return metadata.ExecutionContext.Schema;
        } 

        ISchema IReader.GetSchema()
        {
            return GetSchema();
        }

        /// <summary>
        /// Gets the schema being used by the parser.
        /// </summary>
        /// <returns>The schema being used by the parser.</returns>
        public Task<FixedLengthSchema> GetSchemaAsync()
        {
            return Task.FromResult(metadata.ExecutionContext.Schema);
        }

        Task<ISchema> IReader.GetSchemaAsync()
        {
            return Task.FromResult<ISchema>(metadata.ExecutionContext.Schema);
        }

        /// <summary>
        /// Reads the next record from the file.
        /// </summary>
        /// <returns>True if the next record was parsed; otherwise, false if all files are read.</returns>
        public bool Read()
        {
            if (hasError)
            {
                throw new InvalidOperationException(Resources.ReadingWithErrors);
            }
            metadata.Record = null;
            metadata.Values = null;
            HandleHeader();
            try
            {
                values = ParsePartitions();
                if (values == null)
                {
                    return false;
                }

                ++metadata.LogicalRecordNumber;
                return true;
            }
            catch (FlatFileException)
            {
                hasError = true;
                throw;
            }
        }

        private void HandleHeader()
        {
            if (metadata.PhysicalRecordNumber == 0 && metadata.ExecutionContext.Options.IsFirstRecordHeader)
            {
                SkipInternal();
            }
        }

        private object[] ParsePartitions()
        {
            var rawValues = PartitionWithFilter();
            metadata.Values = rawValues;
            while (rawValues != null)
            {
                var values = ParseValues(rawValues);
                if (values != null)
                {
                    RecordParsed?.Invoke(this, new FixedLengthRecordParsedEventArgs(metadata, values));
                    return values;
                }
                rawValues = PartitionWithFilter();
            }
            return null;
        }

        private string[] PartitionWithFilter()
        {
            var record = ReadWithFilter();
            var rawValues = PartitionRecord(record);
            while (rawValues != null && IsSkipped(rawValues))
            {
                record = ReadWithFilter();
                GetSchema(record);
                rawValues = PartitionRecord(record);
            }
            return rawValues;
        }

        private string ReadWithFilter()
        {
            var record = ReadNextRecord();
            while (record != null && IsSkipped(record))
            {
                record = ReadNextRecord();
            }
            return record;
        }

        /// <summary>
        /// Reads the next record from the file.
        /// </summary>
        /// <returns>True if the next record was parsed; otherwise, false if all files are read.</returns>
        public async ValueTask<bool> ReadAsync()
        {
            if (hasError)
            {
                throw new InvalidOperationException(Resources.ReadingWithErrors);
            }
            metadata.Record = null;
            metadata.Values = null;
            await HandleHeaderAsync().ConfigureAwait(false);
            try
            {
                values = await ParsePartitionsAsync().ConfigureAwait(false);
                if (values == null)
                {
                    return false;
                }

                ++metadata.LogicalRecordNumber;
                return true;
            }
            catch (FlatFileException)
            {
                hasError = true;
                throw;
            }
        }

        private async Task HandleHeaderAsync()
        {
            if (metadata.PhysicalRecordNumber == 0 && metadata.ExecutionContext.Options.IsFirstRecordHeader)
            {
                await SkipAsyncInternal().ConfigureAwait(false);
            }
        }

        private async Task<object[]> ParsePartitionsAsync()
        {
            var rawValues = await PartitionWithFilterAsync().ConfigureAwait(false);
            metadata.Values = rawValues;
            while (rawValues != null)
            {
                var values = ParseValues(rawValues);
                if (values != null)
                {
                    return values;
                }
                rawValues = await PartitionWithFilterAsync().ConfigureAwait(false);
            }
            return null;
        }

        private async ValueTask<string[]> PartitionWithFilterAsync()
        {
            var record = await ReadWithFilterAsync().ConfigureAwait(false);
            var rawValues = PartitionRecord(record);
            while (rawValues != null && IsSkipped(rawValues))
            {
                record = await ReadWithFilterAsync().ConfigureAwait(false);
                rawValues = PartitionRecord(record);
            }
            return rawValues;
        }

        private bool IsSkipped(string[] values)
        {
            if (RecordPartitioned == null)
            {
                return false;
            }
            var e = new FixedLengthRecordPartitionedEventArgs(metadata, values);
            RecordPartitioned(this, e);
            return e.IsSkipped;
        }

        private async Task<string> ReadWithFilterAsync()
        {
            var record = await ReadNextRecordAsync().ConfigureAwait(false);
            while (record != null && IsSkipped(record))
            {
                record = await ReadNextRecordAsync().ConfigureAwait(false);
            }
            return record;
        }

        private bool IsSkipped(string record)
        {
            if (RecordRead == null)
            {
                return false;
            }
            var e = new FixedLengthRecordReadEventArgs(record);
            RecordRead(this, e);
            return e.IsSkipped;
        }

        private object[] ParseValues(string[] rawValues)
        {
            try
            {
                return metadata.ExecutionContext.Schema.ParseValues(metadata, rawValues);
            }
            catch (FlatFileException exception)
            {
                ProcessError(new RecordProcessingException(metadata, Resources.InvalidRecordConversion, exception));
                return null;
            }
        }

        /// <summary>
        /// Skips the next record from the file.
        /// </summary>
        /// <returns>True if the next record was skipped; otherwise, false if all records are read.</returns>
        /// <remarks>The previously parsed values remain available.</remarks>
        public bool Skip()
        {
            if (hasError)
            {
                throw new InvalidOperationException(Resources.ReadingWithErrors);
            }
            HandleHeader();
            return SkipInternal();
        }

        private bool SkipInternal()
        {
            var record = ReadNextRecord();
            return record != null;
        }

        /// <summary>
        /// Skips the next record from the file.
        /// </summary>
        /// <returns>True if the next record was skipped; otherwise, false if all records are read.</returns>
        /// <remarks>The previously parsed values remain available.</remarks>
        public async ValueTask<bool> SkipAsync()
        {
            if (hasError)
            {
                throw new InvalidOperationException(Resources.ReadingWithErrors);
            }
            await HandleHeaderAsync().ConfigureAwait(false);
            return await SkipAsyncInternal().ConfigureAwait(false);
        }

        private async ValueTask<bool> SkipAsyncInternal()
        {
            var record = await ReadNextRecordAsync().ConfigureAwait(false);
            return record != null;
        }

        private string[] PartitionRecord(string record)
        {
            if (record == null)
            {
                return null;
            }
            var schema = GetSchema(record);
            metadata.ExecutionContext.Schema = schema;
            if (schema == null)
            {
                return null;
            }
            if (record.Length < schema.TotalWidth)
            {
                ProcessError(new RecordProcessingException(metadata, Resources.FixedLengthRecordTooShort));
                return null;
            }
            var windows = schema.Windows;
            var values = new string[schema.ColumnDefinitions.Count - schema.ColumnDefinitions.MetadataCount];
            int offset = 0;
            for (int valueIndex = 0, columnIndex = 0; valueIndex != values.Length; ++columnIndex)
            {
                var definition = schema.ColumnDefinitions[columnIndex];
                if (!(definition is IMetadataColumn))
                {
                    Window window = columnIndex < windows.Count ? windows[columnIndex] : null;
                    string value;
                    if (window == null)
                    {
                        value = record.Substring(offset);
                    }
                    else
                    {
                        value = record.Substring(offset, window.Width);
                        var options = metadata.ExecutionContext.Options;
                        var alignment = window.Alignment ?? options.Alignment;
                        value = alignment == FixedAlignment.LeftAligned
                            ? value.TrimEnd(window.FillCharacter ?? options.FillCharacter)
                            : value.TrimStart(window.FillCharacter ?? options.FillCharacter);
                        offset += window.Width;
                    }
                    values[valueIndex] = value;
                    ++valueIndex;
                }
            }
            return values;
        }

        private FixedLengthSchema GetSchema(string record)
        {
            if (record == null || schemaSelector == null)
            {
                return metadata.ExecutionContext.Schema;
            }
            FixedLengthSchema schema = schemaSelector.GetSchema(record);
            if (schema != null)
            {
                return schema;
            }
            ProcessError(new RecordProcessingException(metadata, Resources.MissingMatcher));
            return null;
        }

        private string ReadNextRecord()
        {
            if (parser.IsEndOfStream())
            {
                endOfFile = true;
                return null;
            }
            var record = parser.ReadRecord();
            metadata.Record = record;
            ++metadata.PhysicalRecordNumber;
            return record;
        }

        private async Task<string> ReadNextRecordAsync()
        {
            if (await parser.IsEndOfStreamAsync().ConfigureAwait(false))
            {
                endOfFile = true;
                return null;
            }
            var record = await parser.ReadRecordAsync().ConfigureAwait(false);
            metadata.Record = record;
            ++metadata.PhysicalRecordNumber;
            return record;
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
            if (metadata.PhysicalRecordNumber == 0)
            {
                throw new InvalidOperationException(Resources.ReadNotCalled);
            }
            if (endOfFile)
            {
                throw new InvalidOperationException(Resources.NoMoreRecords);
            }
            var copy = new object[values.Length];
            Array.Copy(values, copy, values.Length);
            return copy;
        }

        IRecordContext IReaderWithMetadata.GetMetadata()
        {
            return metadata;
        }
    }
}
