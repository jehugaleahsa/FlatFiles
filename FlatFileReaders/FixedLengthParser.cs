using System;
using System.Collections.Generic;
using FlatFileReaders.Properties;
using System.IO;

namespace FlatFileReaders
{
    using System.Text;

    /// <summary>
    /// Extracts records from a file that has value in fixed-length columns.
    /// </summary>
    public sealed class FixedLengthParser : IParser
    {
        private readonly Stream stream;
        private readonly string text;
        private readonly FixedLengthSchema schema;
        private readonly string recordSeparator;
        private readonly char filler;
        private int recordCount;
        private object[] values;
        private int nextIndex;
        private bool endOfFile;
        private bool hasError;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of a FixedLengthParser.
        /// </summary>
        /// <param name="fileName">The path of the file containing the records to parse.</param>
        /// <param name="schema">The schema object defining which columns are in each record.</param>
        /// <param name="encoding">The encoding.</param>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        public FixedLengthParser(string fileName, FixedLengthSchema schema, Encoding encoding = null)
            : this(File.OpenRead(fileName), schema, new FixedLengthParserOptions(), encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of a FixedLengthParser.
        /// </summary>
        /// <param name="fileName">The path to the file containing the records to parse.</param>
        /// <param name="schema">The schema object defining which columns are in each record.</param>
        /// <param name="options">An object containing settings for configuring the parser.</param>
        /// <param name="encoding">The encoding.</param>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        public FixedLengthParser(string fileName, FixedLengthSchema schema, FixedLengthParserOptions options, Encoding encoding = null)
            : this(File.OpenRead(fileName), schema, options, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of a FixedLengthParser.
        /// </summary>
        /// <param name="stream">A stream containing the records to parse.</param>
        /// <param name="schema">The schema object defining which columns are in each record.</param>
        /// <param name="encoding">The encoding.</param>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        public FixedLengthParser(Stream stream, FixedLengthSchema schema, Encoding encoding = null)
            : this(stream, schema, new FixedLengthParserOptions(), encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of a FixedLengthParser.
        /// </summary>
        /// <param name="stream">A stream containing the records to parse.</param>
        /// <param name="schema">The schema object defining which columns are in each record.</param>
        /// <param name="options">An object containing settings for configuring the parser.</param>
        /// <param name="encoding">The encoding.</param>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        public FixedLengthParser(Stream stream, FixedLengthSchema schema, FixedLengthParserOptions options, Encoding encoding = null)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (schema == null)
            {
                throw new ArgumentNullException("schema");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            if (encoding == null)
            {
                encoding = Encoding.Default;
            }
            this.stream = stream;
            StreamReader reader = new StreamReader(stream, encoding);
            text = reader.ReadToEnd();
            this.schema = schema;
            recordSeparator = options.RecordSeparator;
            filler = options.FillCharacter;
        }

        /// <summary>
        /// Finalizes the FixedLengthParser.
        /// </summary>
        ~FixedLengthParser()
        {
            dispose(false);
        }

        /// <summary>
        /// Releases any resources currently held by the parser.
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
                stream.Dispose();
            }
            isDisposed = true;
        }

        /// <summary>
        /// Gets the schema being used by the parser.
        /// </summary>
        /// <returns>The schema being used by the parser.</returns>
        Schema IParser.GetSchema()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("FixedLengthParser");
            }
            return schema.Schema;
        }

        /// <summary>
        /// Reads the next record from the file.
        /// </summary>
        /// <returns>True if the next record was parsed; otherwise, false if all files are read.</returns>
        public bool Read()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("FixedLengthParser");
            }
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
            if (isDisposed)
            {
                throw new ObjectDisposedException("FixedLengthParser");
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
    }
}
