using System;
using System.Linq;
using System.Text.RegularExpressions;
using FlatFileReaders.Properties;

namespace FlatFileReaders
{
    /// <summary>
    /// Extracts records from a file that has values separated by a separator token.
    /// </summary>
    public sealed class SeparatedValueParser : IParser
    {
        private string text;
        private int nextIndex;
        private Regex regex;
        private int recordCount;
        private bool endOfFile;
        private bool hasError;
        private Schema schema;
        private object[] values;

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="text">The text containing the records to extract.</param>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        public SeparatedValueParser(string text)
            : this(text, null, new SeparatedValueParserOptions(), false)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="text">The text containing the records to extract.</param>
        /// <param name="schema">The predefined schema for the records.</param>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        public SeparatedValueParser(string text, Schema schema)
            : this(text, schema, new SeparatedValueParserOptions(), true)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="text">The text containing the records to extract.</param>
        /// <param name="options">The options for configuring the parser's behavior.</param>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        /// <exception cref="System.ArgumentNullException">The options object is null.</exception>
        public SeparatedValueParser(string text, SeparatedValueParserOptions options)
            : this(text, null, options, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="text">The text containing the records to extract.</param>
        /// <param name="schema">The predefined schema for the records.</param>
        /// <param name="options">The options for configuring the parser's behavior.</param>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        /// <exception cref="System.ArgumentNullException">The options object is null.</exception>
        public SeparatedValueParser(string text, Schema schema, SeparatedValueParserOptions options)
            : this(text, schema, options, true)
        {
        }

        private SeparatedValueParser(string text, Schema schema, SeparatedValueParserOptions options, bool hasSchema)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (hasSchema && schema == null)
            {
                throw new ArgumentNullException("schema");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            this.text = text;
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
                this.schema = new Schema();
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
        Schema IParser.GetSchema()
        {
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
            if (hasError)
            {
                throw new InvalidOperationException(Resources.ReadingWithErrors);
            }
            if (nextIndex == text.Length)
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
                throw new ParserException(recordCount);
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
            const string doubleQuoteBlock = @"""(?<block>([^""]|(""""))*)""";
            const string singleQuoteBlock = @"'(?<block>([^']|(''))*)'";
            const string noQuoteBlock = @"(?<block>.*?)";
            const string block = @"\s*((" + doubleQuoteBlock + ")|(" + singleQuoteBlock + ")|" + noQuoteBlock + @")\s*?";
            string leading = block + delimiter;
            string trailing = block + @"\r?$";
            Regex regex = new Regex(@"\G(" + leading + ")*(" + trailing + ")", RegexOptions.Compiled | RegexOptions.Multiline);
            return regex;
        }

        private string[] readNextRecord()
        {
            Match match = regex.Match(text, nextIndex);
            Group blockGroup = match.Groups["block"];
            nextIndex = match.Index + match.Length;
            string[] values = blockGroup.Captures.Cast<Capture>().Select(c => c.Value).ToArray();
            return values;
        }
    }
}
