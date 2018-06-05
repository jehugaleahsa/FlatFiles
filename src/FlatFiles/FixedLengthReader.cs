using System;
using System.IO;
using System.Threading.Tasks;
using FlatFiles.Resources;

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
                throw new ArgumentNullException("reader");
            }
            if (hasSchema && schema == null)
            {
                throw new ArgumentNullException("schema");
            }
            if (options == null)
            {
                options = new FixedLengthOptions();
            }
            parser = new FixedLengthRecordParser(reader, schema, options);
            this.metadata = new Metadata()
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
                throw new InvalidOperationException(SharedResources.ReadingWithErrors);
            }
            handleHeader();
            try
            {
                values = parsePartitions();
                if (values == null)
                {
                    return false;
                }
                else
                {
                    ++metadata.LogicalRecordCount;
                    return true;
                }
            }
            catch (FlatFileException)
            {
                hasError = true;
                throw;
            }
        }

        private void handleHeader()
        {
            if (metadata.RecordCount == 0 && metadata.Options.IsFirstRecordHeader)
            {
                skip();
            }
        }

        private object[] parsePartitions()
        {
            (FixedLengthSchema schema, string[] rawValues) = partitionWithFilter();
            while (rawValues != null)
            {
                object[] values = parseValues(schema, rawValues);
                if (values != null)
                {
                    return values;
                }
                (schema, rawValues) = partitionWithFilter();
            }
            return null;
        }

        private ValueTuple<FixedLengthSchema, string[]> partitionWithFilter()
        {
            string record = readWithFilter();
            if (record == null)
            {
                return ValueTuple.Create<FixedLengthSchema, string[]>(null, null);
            }
            var schema = getSchema(record);
            string[] rawValues = partitionRecord(schema, record);
            while (rawValues != null && isSkipped(rawValues))
            {
                record = readWithFilter();
                schema = getSchema(record);
                rawValues = partitionRecord(schema, record);
            }
            return ValueTuple.Create(schema, rawValues);
        }

        private string readWithFilter()
        {
            string record = readNextRecord();
            while (record != null && isSkipped(record))
            {
                record = readNextRecord();
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
                throw new InvalidOperationException(SharedResources.ReadingWithErrors);
            }
            await handleHeaderAsync();
            try
            {
                values = await parsePartitionsAsync();
                if (values == null)
                {
                    return false;
                }
                else
                {
                    ++metadata.LogicalRecordCount;
                    return true;
                }
            }
            catch (FlatFileException)
            {
                hasError = true;
                throw;
            }
        }

        private async Task handleHeaderAsync()
        {
            if (metadata.RecordCount == 0 && metadata.Options.IsFirstRecordHeader)
            {
                await skipAsync();
            }
        }

        private async Task<object[]> parsePartitionsAsync()
        {
            (FixedLengthSchema schema, string[] rawValues) = await partitionWithFilterAsync();
            while (rawValues != null)
            {
                object[] values = parseValues(schema, rawValues);
                if (values != null)
                {
                    return values;
                }
                (schema, rawValues) = await partitionWithFilterAsync();
            }
            return null;
        }

        private async ValueTask<ValueTuple<FixedLengthSchema, string[]>> partitionWithFilterAsync()
        {
            string record = await readWithFilterAsync();
            if (record == null)
            {
                return ValueTuple.Create<FixedLengthSchema, string[]>(null, null);
            }
            var schema = getSchema(record);
            string[] rawValues = partitionRecord(schema, record);
            while (rawValues != null && isSkipped(rawValues))
            {
                record = await readWithFilterAsync();
                schema = getSchema(record);
                rawValues = partitionRecord(schema, record);
            }
            return ValueTuple.Create(schema, rawValues);
        }

        private FixedLengthSchema getSchema(string record)
        {
            return schemaSelector == null ? metadata.Schema : schemaSelector.GetSchema(record);
        }

        private bool isSkipped(string[] values)
        {
            if (RecordPartitioned == null)
            {
                return false;
            }
            var e = new FixedLengthRecordPartitionedEventArgs(values);
            RecordPartitioned(this, e);
            return e.IsSkipped;
        }

        private async Task<string> readWithFilterAsync()
        {
            string record = await readNextRecordAsync();
            while (record != null && isSkipped(record))
            {
                record = await readNextRecordAsync();
            }
            return record;
        }

        private bool isSkipped(string record)
        {
            if (RecordRead == null)
            {
                return false;
            }
            var e = new FixedLengthRecordReadEventArgs(record);
            RecordRead(this, e);
            return e.IsSkipped;
        }

        private object[] parseValues(FixedLengthSchema schema, string[] rawValues)
        {
            try
            {
                var metadata = schemaSelector == null ? this.metadata : new Metadata()
                {
                    Schema = schema,
                    Options = this.metadata.Options,
                    RecordCount = this.metadata.RecordCount,
                    LogicalRecordCount = this.metadata.LogicalRecordCount
                };
                return schema.ParseValues(metadata, rawValues);
            }
            catch (FlatFileException exception)
            {
                processError(new RecordProcessingException(metadata.RecordCount, SharedResources.InvalidRecordConversion, exception));
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
                throw new InvalidOperationException(SharedResources.ReadingWithErrors);
            }
            handleHeader();
            return skip();
        }

        private bool skip()
        {
            string record = readNextRecord();
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
                throw new InvalidOperationException(SharedResources.ReadingWithErrors);
            }
            await handleHeaderAsync();
            return await skipAsync();
        }

        private async ValueTask<bool> skipAsync()
        {
            string record = await readNextRecordAsync();
            return record != null;
        }

        private string[] partitionRecord(FixedLengthSchema schema, string record)
        {
            if (record.Length < schema.TotalWidth)
            {
                processError(new RecordProcessingException(metadata.RecordCount, SharedResources.FixedLengthRecordTooShort));
                return null;
            }
            WindowCollection windows = schema.Windows;
            string[] values = new string[windows.Count - schema.ColumnDefinitions.MetadataCount];
            int offset = 0;
            for (int index = 0; index != values.Length;)
            {
                var definition = schema.ColumnDefinitions[index];
                if (!(definition is IMetadataColumn metaColumn))
                {
                    Window window = windows[index];
                    string value = record.Substring(offset, window.Width);
                    var alignment = window.Alignment ?? metadata.Options.Alignment;
                    if (alignment == FixedAlignment.LeftAligned)
                    {
                        value = value.TrimEnd(window.FillCharacter ?? metadata.Options.FillCharacter);
                    }
                    else
                    {
                        value = value.TrimStart(window.FillCharacter ?? metadata.Options.FillCharacter);
                    }
                    values[index] = value;
                    ++index;
                    offset += window.Width;
                }
            }
            return values;
        }

        private string readNextRecord()
        {
            if (parser.IsEndOfStream())
            {
                endOfFile = true;
                return null;
            }
            string record = parser.ReadRecord();
            ++metadata.RecordCount;
            return record;
        }

        private async Task<string> readNextRecordAsync()
        {
            if (await parser.IsEndOfStreamAsync())
            {
                endOfFile = true;
                return null;
            }
            string record = await parser.ReadRecordAsync();
            ++metadata.RecordCount;
            return record;
        }

        private void processError(RecordProcessingException exception)
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
                throw new InvalidOperationException(SharedResources.ReadingWithErrors);
            }
            if (metadata.RecordCount == 0)
            {
                throw new InvalidOperationException(SharedResources.ReadNotCalled);
            }
            if (endOfFile)
            {
                throw new InvalidOperationException(SharedResources.NoMoreRecords);
            }
            object[] copy = new object[values.Length];
            Array.Copy(values, copy, values.Length);
            return copy;
        }

        private class Metadata : IProcessMetadata
        {
            public FixedLengthSchema Schema { get; internal set; }

            ISchema IProcessMetadata.Schema
            {
                get { return Schema; }
            }

            public FixedLengthOptions Options { get; internal set; }

            IOptions IProcessMetadata.Options
            {
                get { return Options; }
            }

            public int RecordCount { get; internal set; }

            public int LogicalRecordCount { get; internal set; }
        }
    }
}
