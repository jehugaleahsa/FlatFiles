using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FlatFileReaders.Properties;

namespace FlatFileReaders
{
    /// <summary>
    /// Extracts records from a stream that are separated by a separator string.
    /// </summary>
    public sealed class SeparatedValueParser
    {
        private string text;
        private int nextIndex;
        private Regex regex;
        private int recordCount;
        private bool endOfFile;
        private string[] schema;
        private string[] values;

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="stream">The stream containing the records to extract.</param>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        public SeparatedValueParser(Stream stream)
            : this(stream, new SeparatedValueParserOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="stream">The stream containing the records to extract.</param>
        /// <param name="options">The options for configuring the parser's behavior.</param>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        /// <exception cref="System.ArgumentNullException">The options object is null.</exception>
        public SeparatedValueParser(Stream stream, SeparatedValueParserOptions options)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            StreamReader reader = new StreamReader(stream);
            text = reader.ReadToEnd();
            regex = buildRegex(options.Separator);
            if (options.IsFirstRecordSchema)
            {
                schema = readNextRecord();
            }
        }

        /// <summary>
        /// Gets the names of the columns found in the file.
        /// </summary>
        /// <returns>The names.</returns>
        public string[] GetSchema()
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
        /// <exception cref="ParserException">The parser encountered an invalid record format.</exception>
        public bool Read()
        {
            if (nextIndex == text.Length)
            {
                endOfFile = true;
                values = null;
                return false;
            }
            values = readNextRecord();
            recordCount += 1;
            return true;
        }

        /// <summary>
        /// Gets the values for the current record.
        /// </summary>
        /// <returns>The values of the current record.</returns>
        public string[] GetValues()
        {
            if (recordCount == 0)
            {
                throw new InvalidOperationException(Resources.ReadNotCalled);
            }
            if (endOfFile)
            {
                throw new InvalidOperationException(Resources.NoMoreRecords);
            }
            string[] copy = new string[values.Length];
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
