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
        private readonly FixedLengthSchema schema;
        private readonly FixedLengthOptions options;
        private int recordCount;
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
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (schema == null)
            {
                throw new ArgumentNullException("schema");
            }
            if (options == null)
            {
                options = new FixedLengthOptions();
            }
            parser = new FixedLengthRecordParser(reader, schema, options);
            this.schema = schema;
            this.options = options.Clone();
        }

        /// <summary>
        /// Gets the schema being used by the parser.
        /// </summary>
        /// <returns>The schema being used by the parser.</returns>
        public FixedLengthSchema GetSchema()
        {
            return schema;
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
            return Task.FromResult(schema);
        }

        Task<ISchema> IReader.GetSchemaAsync()
        {
            return Task.FromResult<ISchema>(schema);
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
                return values != null;
            }
            catch (FlatFileException)
            {
                hasError = true;
                throw;
            }
        }

        private void handleHeader()
        {
            if (recordCount == 0 && options.IsFirstRecordHeader)
            {
                skip();
            }
        }

        private object[] parsePartitions()
        {
            string[] rawValues = partitionWithFilter();
            while (rawValues != null)
            {
                object[] values = parseValues(rawValues);
                if (values != null)
                {
                    return values;
                }
                rawValues = partitionWithFilter();
            }
            return null;
        }

        private string[] partitionWithFilter()
        {
            string record = readWithFilter();
            string[] rawValues = partitionRecord(record);
            while (rawValues != null && options.PartitionedRecordFilter != null && options.PartitionedRecordFilter(rawValues))
            {
                record = readWithFilter();
                rawValues = partitionRecord(record);
            }
            return rawValues;
        }

        private string readWithFilter()
        {
            string record = readNextRecord();
            while (record != null && options.UnpartitionedRecordFilter != null && options.UnpartitionedRecordFilter(record))
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
                return values != null;
            }
            catch (FlatFileException)
            {
                hasError = true;
                throw;
            }
        }

        private async Task handleHeaderAsync()
        {
            if (recordCount == 0 && options.IsFirstRecordHeader)
            {
                await skipAsync();
            }
        }

        private async Task<object[]> parsePartitionsAsync()
        {
            string[] rawValues = await partitionWithFilterAsync();
            while (rawValues != null)
            {
                object[] values = parseValues(rawValues);
                if (values != null)
                {
                    return values;
                }
                rawValues = await partitionWithFilterAsync();
            }
            return null;
        }

        private async Task<string[]> partitionWithFilterAsync()
        {
            string record = await readWithFilterAsync();
            string[] rawValues = partitionRecord(record);
            while (rawValues != null && options.PartitionedRecordFilter != null && options.PartitionedRecordFilter(rawValues))
            {
                record = await readWithFilterAsync();
                rawValues = partitionRecord(record);
            }
            return rawValues;
        }

        private async Task<string> readWithFilterAsync()
        {
            string record = await readNextRecordAsync();
            while (record != null && options.UnpartitionedRecordFilter != null && options.UnpartitionedRecordFilter(record))
            {
                record = await readNextRecordAsync();
            }
            return record;
        }

        private object[] parseValues(string[] rawValues)
        {
            try
            {
                return schema.ParseValues(rawValues);
            }
            catch (FlatFileException exception)
            {
                processError(new RecordProcessingException(recordCount, SharedResources.InvalidRecordConversion, exception));
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

        private string[] partitionRecord(string record)
        {
            if (record == null)
            {
                return null;
            }
            if (record.Length < schema.TotalWidth)
            {
                processError(new RecordProcessingException(recordCount, SharedResources.FixedLengthRecordTooShort));
                return null;
            }
            WindowCollection windows = schema.Windows;
            string[] values = new string[windows.Count];
            int offset = 0;
            for (int index = 0; index != values.Length; ++index)
            {
                Window window = windows[index];
                string value = record.Substring(offset, window.Width);
                var alignment = window.Alignment ?? options.Alignment;
                if (alignment == FixedAlignment.LeftAligned)
                {
                    value = value.TrimEnd(window.FillCharacter ?? options.FillCharacter);
                }
                else
                {
                    value = value.TrimStart(window.FillCharacter ?? options.FillCharacter);
                }
                values[index] = value;
                offset += window.Width;
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
            ++recordCount;
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
            ++recordCount;
            return record;
        }

        private void processError(RecordProcessingException exception)
        {
            if (options.ErrorHandler != null)
            {
                var args = new ProcessingErrorEventArgs(exception);
                options.ErrorHandler(this, args);
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
            if (recordCount == 0)
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
    }
}
