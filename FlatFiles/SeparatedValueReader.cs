using System;
using System.Linq;
using System.Text.RegularExpressions;
using FlatFiles.Properties;
using System.IO;

namespace FlatFiles
{
    using System.Text;

    /// <summary>
    /// Extracts records from a file that has values separated by a separator token.
    /// </summary>
    public sealed class SeparatedValueReader : IReader
    {
        private readonly RecordReader reader;
        private readonly SeparatedValueSchema schema;
        private readonly Regex regex;
        private int recordCount;
        private object[] values;
        private bool endOfFile;
        private bool hasError;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="fileName">The path of the file containing the records to parse.</param>
        public SeparatedValueReader(string fileName)
            : this(File.OpenRead(fileName), null, new SeparatedValueOptions(), false, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="fileName">The path of the file containing the records to extract.</param>
        /// <param name="schema">The predefined schema for the records.</param>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        public SeparatedValueReader(string fileName, SeparatedValueSchema schema)
            : this(File.OpenRead(fileName), schema, new SeparatedValueOptions(), true, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="fileName">The path of the file containing the records to extract.</param>
        /// <param name="options">The options for configuring the parser's behavior.</param>
        /// <exception cref="System.ArgumentNullException">The options object is null.</exception>
        public SeparatedValueReader(string fileName, SeparatedValueOptions options)
            : this(File.OpenRead(fileName), null, options, false, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="fileName">The path of the file containing the records to extract.</param>
        /// <param name="schema">The predefined schema for the records.</param>
        /// <param name="options">The options for configuring the parser's behavior.</param>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        /// <exception cref="System.ArgumentNullException">The options object is null.</exception>
        public SeparatedValueReader(string fileName, SeparatedValueSchema schema, SeparatedValueOptions options)
            : this(File.OpenRead(fileName), schema, options, true, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="stream">A stream containing the records to parse.</param>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        public SeparatedValueReader(Stream stream)
            : this(stream, null, new SeparatedValueOptions(), false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="stream">A stream containing the records to parse.</param>
        /// <param name="schema">The predefined schema for the records.</param>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        public SeparatedValueReader(Stream stream, SeparatedValueSchema schema)
            : this(stream, schema, new SeparatedValueOptions(), true, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="stream">A stream containing the records to parse.</param>
        /// <param name="options">The options for configuring the parser's behavior.</param>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        /// <exception cref="System.ArgumentNullException">The options is null.</exception>
        public SeparatedValueReader(Stream stream, SeparatedValueOptions options)
            : this(stream, null, options, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="stream">A stream containing the records to parse.</param>
        /// <param name="schema">The predefined schema for the records.</param>
        /// <param name="options">The options for configuring the parser's behavior.</param>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        /// <exception cref="System.ArgumentNullException">The options is null.</exception>
        public SeparatedValueReader(Stream stream, SeparatedValueSchema schema, SeparatedValueOptions options)
            : this(stream, schema, options, true, false)
        {
        }

        private SeparatedValueReader(Stream stream, SeparatedValueSchema schema, SeparatedValueOptions options, bool hasSchema, bool ownsStream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (hasSchema && schema == null)
            {
                throw new ArgumentNullException("schema");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            reader = new RecordReader(stream, options.Encoding, options.RecordSeparator, ownsStream);
            regex = buildRegex(options.Separator);
            if (hasSchema)
            {
                if (options.IsFirstRecordSchema)
                {
                    readNextRecord();  // skip header record
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
        /// Finalizes the SeparatedValueParser.
        /// </summary>
        ~SeparatedValueReader()
        {
            dispose(false);
        }

        /// <summary>
        /// Releases any resources being held by the parser.
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        private void dispose(bool disposing)
        {
            if (disposing)
            {
                reader.Dispose();
            }
            isDisposed = true;
        }

        /// <summary>
        /// Gets the names of the columns found in the file.
        /// </summary>
        /// <returns>The names.</returns>
        public ISchema GetSchema()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("SeparatedValueReader");
            }
            if (schema == null)
            {
                throw new InvalidOperationException(Resources.SchemaNotDefined);
            }
            return schema;
        }

        /// <summary>
        /// Attempts to read the next record from the stream.
        /// </summary>
        /// <returns>True if the next record was read or false if all records have been read.</returns>
        public bool Read()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("SeparatedValueReader");
            }
            if (hasError)
            {
                throw new InvalidOperationException(Resources.ReadingWithErrors);
            }
            if (reader.EndOfStream)
            {
                endOfFile = true;
                values = null;
                return false;
            }
            string[] rawValues = readNextRecord();
            recordCount += 1;
            if (schema == null)
            {
                values = rawValues;
            }
            else if (rawValues.Length != schema.ColumnDefinitions.Count)
            {
                hasError = true;
                throw new FlatFileException(recordCount);
            }
            else
            {
                values = schema.ParseValues(rawValues);
            }
            return true;
        }

        /// <summary>
        /// Gets the values for the current record.
        /// </summary>
        /// <returns>The values of the current record.</returns>
        public object[] GetValues()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("SeparatedValueReader");
            }
            if (hasError)
            {
                throw new InvalidOperationException(Resources.ReadingWithErrors);
            }
            if (recordCount == 0)
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

        private static Regex buildRegex(string delimiter)
        {
            delimiter = Regex.Escape(delimiter);
            const string singleQuoteBlock = @"(?:'(?<block>(?:(?:[^'])|(?:''))*?)')";
            const string doubleQuoteBlock = @"(?:""(?<block>(?:(?:[^""])|(?:""""))*?)"")";
            const string noQuoteBlock = @"(?<block>[^'""]*?)";
            const string block = @"(?:" + singleQuoteBlock + @"|" + doubleQuoteBlock + @"|" + noQuoteBlock + @")";
            string leading = block + delimiter;
            string trailing = block + @"\r?$";
            Regex regex = new Regex(@"\G(?:" + leading + ")*(?:" + trailing + ")", RegexOptions.Multiline);
            return regex;
        }

        private string[] readNextRecord()
        {
            string record = reader.ReadRecord();
            Match match = regex.Match(record);
            Group blockGroup = match.Groups["block"];
            string[] values = blockGroup.Captures.Cast<Capture>().Select(c => c.Value).ToArray();
            return values;
        }
    }
}
