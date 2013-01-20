using System;
using System.Collections.Generic;
using FlatFileReaders.Properties;

namespace FlatFileReaders
{
    /// <summary>
    /// Extracts records from a file that has value in fixed-length columns.
    /// </summary>
    public sealed class FixedLengthParser : IParser
    {
        private readonly string text;
        private readonly FixedLengthSchema schema;
        private int recordCount;
        private int nextIndex;
        private bool endOfFile;
        private bool hasError;
        private char filler;
        private string recordSeparator;
        private object[] values;

        /// <summary>
        /// Initializes a new instance of a FixedLengthParser.
        /// </summary>
        /// <param name="text">The file containing the records to parse.</param>
        /// <param name="schema">The schema object defining which columns are in each record.</param>
        /// <exception cref="System.ArgumentNullException">The text is null.</exception>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        public FixedLengthParser(string text, FixedLengthSchema schema)
            : this(text, schema, new FixedLengthParserOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of a FixedLengthParser.
        /// </summary>
        /// <param name="text">The file containing the records to parse.</param>
        /// <param name="schema">The schema object defining which columns are in each record.</param>
        /// <param name="options">An object for configuring the parser.</param>
        /// <exception cref="System.ArgumentNullException">The text is null.</exception>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        /// <exception cref="System.ArgumentNullException">The options object is null.</exception>
        public FixedLengthParser(string text, FixedLengthSchema schema, FixedLengthParserOptions options)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (schema == null)
            {
                throw new ArgumentNullException("schema");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            this.text = text;
            this.schema = schema;
            recordSeparator = options.RecordSeparator;
            filler = options.FillCharacter;
        }

        /// <summary>
        /// Gets the schema being used by the parser.
        /// </summary>
        /// <returns>The schema being used by the parser.</returns>
        Schema IParser.GetSchema()
        {
            return schema.Schema;
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
            if (nextIndex == text.Length)
            {
                endOfFile = true;
                return false;
            }
            ++recordCount;
            string[] rawValues = readNextLine();
            values = schema.ParseValues(rawValues);
            return true;
        }

        private string[] readNextLine()
        {
            string line = getNextLine();
            if (line.Length != schema.TotalWidth)
            {
                hasError = true;
                throw new ParserException(recordCount);
            }
            List<int> widths = schema.ColumnWidths;
            string[] values = new string[schema.ColumnWidths.Count];
            int offset = 0;
            for (int index = 0; index != values.Length; ++index)
            {
                int width = widths[index];
                values[index] = line.Substring(offset, width).Trim(filler);
                offset += width;
            }
            return values;
        }

        private string getNextLine()
        {
            int index = text.IndexOf(recordSeparator, nextIndex);
            if (index == -1)
            {
                string line = text.Substring(nextIndex);
                nextIndex = text.Length;
                return line;
            }
            else
            {
                string line = text.Substring(nextIndex, index - nextIndex);
                nextIndex = index + recordSeparator.Length;
                return line;
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
    }
}
