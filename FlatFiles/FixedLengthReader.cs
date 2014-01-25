using System;
using System.Collections.Generic;
using FlatFiles.Properties;
using System.IO;

namespace FlatFiles
{
    using System.Text;

    /// <summary>
    /// Extracts records from a file that has value in fixed-length columns.
    /// </summary>
    public sealed class FixedLengthReader : IReader
    {
        private readonly StreamReader reader;
        private readonly bool ownsStream;
        private readonly FixedLengthSchema schema;
        private readonly string recordSeparator;
        private readonly char filler;
        private int recordCount;
        private object[] values;
        private bool endOfFile;
        private bool hasError;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of a FixedLengthParser.
        /// </summary>
        /// <param name="fileName">The path of the file containing the records to parse.</param>
        /// <param name="schema">The schema object defining which columns are in each record.</param>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        public FixedLengthReader(string fileName, FixedLengthSchema schema)
            : this(File.OpenRead(fileName), schema, new FixedLengthOptions(), true)
        {
        }

        /// <summary>
        /// Initializes a new instance of a FixedLengthParser.
        /// </summary>
        /// <param name="fileName">The path to the file containing the records to parse.</param>
        /// <param name="schema">The schema object defining which columns are in each record.</param>
        /// <param name="options">An object containing settings for configuring the parser.</param>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        /// <exception cref="System.ArgumentNullException">The options is null.</exception>
        public FixedLengthReader(string fileName, FixedLengthSchema schema, FixedLengthOptions options)
            : this(File.OpenRead(fileName), schema, options, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of a FixedLengthParser.
        /// </summary>
        /// <param name="stream">A stream containing the records to parse.</param>
        /// <param name="schema">The schema object defining which columns are in each record.</param>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        public FixedLengthReader(Stream stream, FixedLengthSchema schema)
            : this(stream, schema, new FixedLengthOptions(), false)
        {
        }

        /// <summary>
        /// Initializes a new instance of a FixedLengthParser.
        /// </summary>
        /// <param name="stream">A stream containing the records to parse.</param>
        /// <param name="schema">The schema object defining which columns are in each record.</param>
        /// <param name="options">An object containing settings for configuring the parser.</param>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        /// <exception cref="System.ArgumentNullException">The options is null.</exception>
        public FixedLengthReader(Stream stream, FixedLengthSchema schema, FixedLengthOptions options)
            : this(stream, schema, options, false)
        {
        }

        private FixedLengthReader(Stream stream, FixedLengthSchema schema, FixedLengthOptions options, bool ownsStream)
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
            reader = new StreamReader(new BufferedStream(stream), options.Encoding ?? Encoding.Default);
            this.ownsStream = ownsStream;
            this.schema = schema;
            recordSeparator = options.RecordSeparator;
            filler = options.FillCharacter;
        }

        /// <summary>
        /// Finalizes the FixedLengthParser.
        /// </summary>
        ~FixedLengthReader()
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
            if (disposing && ownsStream)
            {
                reader.Dispose();
            }
            isDisposed = true;
        }

        /// <summary>
        /// Gets the schema being used by the parser.
        /// </summary>
        /// <returns>The schema being used by the parser.</returns>
        public ISchema GetSchema()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("FixedLengthParser");
            }
            return schema;
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
            if (reader.EndOfStream)
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
                throw new FlatFileException(recordCount);
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
            List<char> buffer = new List<char>();
            int positionIndex = 0;
            while (!reader.EndOfStream && positionIndex != recordSeparator.Length)
            {
                int value = reader.Read();
                if (value != -1)
                {
                    char next = (char)value;
                    if (next == recordSeparator[positionIndex])
                    {
                        ++positionIndex;
                    }
                    else
                    {
                        positionIndex = 0;
                    }
                    buffer.Add(next);
                }
            }
            if (positionIndex == recordSeparator.Length)
            {
                buffer.RemoveRange(buffer.Count - recordSeparator.Length, recordSeparator.Length);
            }
            return new String(buffer.ToArray());
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
