using System;
using System.IO;
using System.Threading.Tasks;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Extracts records from a file that has value in fixed-length columns.
    /// </summary>
    public sealed class FixedLengthReader : IReader
    {
        private readonly FixedLengthRecordParser parser;
        private readonly FixedLengthSchemaSelector schemaSelector;
        private readonly Metadata metadata;
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
            metadata = new Metadata()
            {
                Schema = schema,
                Options = options.Clone()
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
        public event EventHandler<ProcessingErrorEventArgs> Error;

        /// <summary>
        /// Gets the schema being used by the parser.
        /// </summary>
        /// <returns>The schema being used by the parser.</returns>
        public FixedLengthSchema GetSchema()
        {
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
        public Task<FixedLengthSchema> GetSchemaAsync()
        {
            return Task.FromResult(metadata.Schema);
        }

        Task<ISchema> IReader.GetSchemaAsync()
        {
            return Task.FromResult<ISchema>(metadata.Schema);
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
            HandleHeader();
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

        private void HandleHeader()
        {
            if (metadata.RecordCount == 0 && metadata.Options.IsFirstRecordHeader)
            {
                skip();
            }
        }

        private object[] ParsePartitions()
        {
            var (schema, rawValues) = PartitionWithFilter();
            while (rawValues != null)
            {
                var values = ParseValues(schema, rawValues);
                if (values != null)
                {
                    RecordParsed?.Invoke(this, new FixedLengthRecordParsedEventArgs(GetMetadata(schema), values));
                    return values;
                }
                (schema, rawValues) = PartitionWithFilter();
            }
            return null;
        }

        private (FixedLengthSchema, string[]) PartitionWithFilter()
        {
            var record = ReadWithFilter();
            var (schema, rawValues) = PartitionRecord(record);
            while (rawValues != null && IsSkipped(schema, rawValues))
            {
                record = ReadWithFilter();
                GetSchema(record);
                (schema, rawValues) = PartitionRecord(record);
            }
            return (schema, rawValues);
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
            await HandleHeaderAsync().ConfigureAwait(false);
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

        private async Task HandleHeaderAsync()
        {
            if (metadata.RecordCount == 0 && metadata.Options.IsFirstRecordHeader)
            {
                await skipAsync().ConfigureAwait(false);
            }
        }

        private async Task<object[]> ParsePartitionsAsync()
        {
            var (schema, rawValues) = await PartitionWithFilterAsync().ConfigureAwait(false);
            while (rawValues != null)
            {
                var values = ParseValues(schema, rawValues);
                if (values != null)
                {
                    return values;
                }
                (schema, rawValues) = await PartitionWithFilterAsync().ConfigureAwait(false);
            }
            return null;
        }

        private async ValueTask<(FixedLengthSchema, string[])> PartitionWithFilterAsync()
        {
            var record = await ReadWithFilterAsync().ConfigureAwait(false);
            var (schema, rawValues) = PartitionRecord(record);
            while (rawValues != null && IsSkipped(schema, rawValues))
            {
                record = await ReadWithFilterAsync().ConfigureAwait(false);
                (schema, rawValues) = PartitionRecord(record);
            }
            return (schema, rawValues);
        }

        private FixedLengthSchema GetSchema(string record)
        {
            if (record == null)
            {
                return null;
            }
            if (schemaSelector == null)
            {
                return metadata.Schema;
            }
            return schemaSelector.GetSchema(record);
        }

        private bool IsSkipped(FixedLengthSchema schema, string[] values)
        {
            if (RecordPartitioned == null)
            {
                return false;
            }
            var metadata = GetMetadata(schema);
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

        private object[] ParseValues(FixedLengthSchema schema, string[] rawValues)
        {
            try
            {
                var metadata = GetMetadata(schema);
                return schema.ParseValues(metadata, rawValues);
            }
            catch (FlatFileException exception)
            {
                ProcessError(new RecordProcessingException(metadata.RecordCount, Resources.InvalidRecordConversion, exception));
                return null;
            }
        }

        private Metadata GetMetadata(FixedLengthSchema schema)
        {
            return schemaSelector == null ? this.metadata : new Metadata()
            {
                Schema = schema,
                Options = this.metadata.Options,
                RecordCount = this.metadata.RecordCount,
                LogicalRecordCount = this.metadata.LogicalRecordCount
            };
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
            return skip();
        }

        private bool skip()
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
            return await skipAsync().ConfigureAwait(false);
        }

        private async ValueTask<bool> skipAsync()
        {
            var record = await ReadNextRecordAsync().ConfigureAwait(false);
            return record != null;
        }

        private (FixedLengthSchema schema, string[]) PartitionRecord(string record)
        {
            var schema = GetSchema(record);
            if (schema == null)
            {
                return (null, null);
            }
            if (record.Length < schema.TotalWidth)
            {
                ProcessError(new RecordProcessingException(metadata.RecordCount, Resources.FixedLengthRecordTooShort));
                return (null, null);
            }
            var windows = schema.Windows;
            var values = new string[windows.Count - schema.ColumnDefinitions.MetadataCount];
            int offset = 0;
            for (int index = 0; index != values.Length;)
            {
                var definition = schema.ColumnDefinitions[index];
                if (!(definition is IMetadataColumn))
                {
                    Window window = windows[index];
                    string value = record.Substring(offset, window.Width);
                    var alignment = window.Alignment ?? metadata.Options.Alignment;
                    value = alignment == FixedAlignment.LeftAligned 
                        ? value.TrimEnd(window.FillCharacter ?? metadata.Options.FillCharacter) 
                        : value.TrimStart(window.FillCharacter ?? metadata.Options.FillCharacter);
                    values[index] = value;
                    ++index;
                    offset += window.Width;
                }
            }
            return (schema, values);
        }

        private string ReadNextRecord()
        {
            if (parser.IsEndOfStream())
            {
                endOfFile = true;
                return null;
            }
            var record = parser.ReadRecord();
            ++metadata.RecordCount;
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
            ++metadata.RecordCount;
            return record;
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
            var copy = new object[values.Length];
            Array.Copy(values, copy, values.Length);
            return copy;
        }

        private class Metadata : IProcessMetadata
        {
            public FixedLengthSchema Schema { get; internal set; }

            ISchema IProcessMetadata.Schema => Schema;

            public FixedLengthOptions Options { get; internal set; }

            IOptions IProcessMetadata.Options => Options;

            public int RecordCount { get; internal set; }

            public int LogicalRecordCount { get; internal set; }
        }
    }
}
