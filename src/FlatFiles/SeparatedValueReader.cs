using System;
using System.IO;
using FlatFiles.Resources;

namespace FlatFiles
{
    /// <summary>
    /// Extracts records from a file that has values separated by a separator token.
    /// </summary>
    public sealed class SeparatedValueReader : IReader
    {
        private readonly SeparatedValueRecordParser parser;
        private readonly SeparatedValueSchema schema;
        private int recordCount;
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

        private SeparatedValueReader(TextReader reader, SeparatedValueSchema schema, SeparatedValueOptions options, bool hasSchema)
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
                options = new SeparatedValueOptions();
            }
            if (options.RecordSeparator == options.Separator)
            {
                throw new ArgumentException(SharedResources.SameSeparator, "options");
            }
            RetryReader retryReader = new RetryReader(reader);
            this.parser = new SeparatedValueRecordParser(retryReader, options);
            if (hasSchema)
            {
                if (options.IsFirstRecordSchema)
                {
                    skip();  // skip header record
                }
                this.schema = schema;
            }
            else if (options.IsFirstRecordSchema)
            {
                string[] columnNames = readNextRecord();
                this.schema = new SeparatedValueSchema();
                foreach (string columnName in columnNames)
                {
                    StringColumn column = new StringColumn(columnName);
                    this.schema.AddColumn(column);
                }
            }
        }

        /// <summary>
        /// Gets the names of the columns found in the file.
        /// </summary>
        /// <returns>The names.</returns>
        public SeparatedValueSchema GetSchema()
        {
            if (schema == null)
            {
                throw new InvalidOperationException(SharedResources.SchemaNotDefined);
            }
            return schema;
        }

        ISchema IReader.GetSchema()
        {
            return GetSchema();
        }

        /// <summary>
        /// Attempts to read the next record from the stream.
        /// </summary>
        /// <returns>True if the next record was read or false if all records have been read.</returns>
        public bool Read()
        {
            if (hasError)
            {
                throw new InvalidOperationException(SharedResources.ReadingWithErrors);
            }
            string[] rawValues = readWithFilter();
            if (rawValues == null)
            {
                return false;
            }
            if (schema == null)
            {
                values = rawValues;
            }
            else if (rawValues.Length < schema.ColumnDefinitions.HandledCount)
            {
                hasError = true;
                throw new RecordProcessingException(recordCount, SharedResources.SeparatedValueRecordWrongNumberOfColumns);
            }
            else
            {
                values = parseValues(rawValues);
            }
            return true;
        }

        private string[] readWithFilter()
        {
            string[] rawValues = readNextRecord();
            while (rawValues != null && parser.Options.PartitionedRecordFilter != null && parser.Options.PartitionedRecordFilter(rawValues))
            {
                rawValues = readNextRecord();
            }
            return rawValues;
        }

        private object[] parseValues(string[] rawValues)
        {
            try
            {
                return schema.ParseValues(rawValues);
            }
            catch (FlatFileException exception)
            {
                throw new RecordProcessingException(recordCount, SharedResources.InvalidRecordConversion, exception);
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
                throw new InvalidOperationException(SharedResources.ReadingWithErrors);
            }
            bool result = skip();
            return result;
        }

        private bool skip()
        {
            string[] rawValues = readNextRecord();
            return rawValues != null;
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

        private string[] readNextRecord()
        {
            if (parser.EndOfStream)
            {
                endOfFile = true;
                values = null;
                return null;
            }
            try
            {
                string[] results = parser.ReadRecord();
                ++recordCount;
                return results;
            }
            catch (SeparatedValueSyntaxException exception)
            {
                throw new RecordProcessingException(recordCount, SharedResources.InvalidRecordFormatNumber, exception);
            }
        }
    }
}
