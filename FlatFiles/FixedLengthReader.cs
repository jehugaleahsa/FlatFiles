using System;
using FlatFiles.Properties;
using System.IO;

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
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of a FixedLengthParser.
        /// </summary>
        /// <param name="fileName">The path of the file containing the records to parse.</param>
        /// <param name="schema">The schema object defining which columns are in each record.</param>
        /// <exception cref="ArgumentNullException">The schema is null.</exception>
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
        /// <exception cref="ArgumentNullException">The schema is null.</exception>
        /// <exception cref="ArgumentNullException">The options is null.</exception>
        public FixedLengthReader(string fileName, FixedLengthSchema schema, FixedLengthOptions options)
            : this(File.OpenRead(fileName), schema, options, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of a FixedLengthParser.
        /// </summary>
        /// <param name="stream">A stream containing the records to parse.</param>
        /// <param name="schema">The schema object defining which columns are in each record.</param>
        /// <exception cref="ArgumentNullException">The stream is null.</exception>
        /// <exception cref="ArgumentNullException">The schema is null.</exception>
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
        /// <exception cref="ArgumentNullException">The stream is null.</exception>
        /// <exception cref="ArgumentNullException">The schema is null.</exception>
        /// <exception cref="ArgumentNullException">The options is null.</exception>
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
            parser = new FixedLengthRecordParser(stream, options, ownsStream);
            this.schema = schema;
            this.options = options.Clone();
            if (this.options.IsFirstRecordHeader)
            {
                skip();
            }
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
            if (disposing)
            {
                parser.Dispose();
            }
            isDisposed = true;
        }

        /// <summary>
        /// Gets the schema being used by the parser.
        /// </summary>
        /// <returns>The schema being used by the parser.</returns>
        public FixedLengthSchema GetSchema()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("FixedLengthReader");
            }
            return schema;
        } 

        ISchema IReader.GetSchema()
        {
            return GetSchema();
        }

        /// <summary>
        /// Reads the next record from the file.
        /// </summary>
        /// <returns>True if the next record was parsed; otherwise, false if all files are read.</returns>
        public bool Read()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("FixedLengthReader");
            }
            if (hasError)
            {
                throw new InvalidOperationException(Resources.ReadingWithErrors);
            }
            if (parser.EndOfStream)
            {
                endOfFile = true;
                return false;
            }
            string[] rawValues = readNextLine();
            values = schema.ParseValues(rawValues, parser.Encoding);
            return true;
        }

        /// <summary>
        /// Skips the next record from the file.
        /// </summary>
        /// <returns>True if the next record was skipped; otherwise, false if all records are read.</returns>
        /// <remarks>The previously parsed values remain available.</remarks>
        public bool Skip()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("FixedLengthReader");
            }
            if (hasError)
            {
                throw new InvalidOperationException(Resources.ReadingWithErrors);
            }
            return skip();
        }

        private bool skip()
        {
            if (parser.EndOfStream)
            {
                endOfFile = true;
                return false;
            }
            parser.ReadRecord();
            ++recordCount;
            return true;
        }

        private string[] readNextLine()
        {
            string record = parser.ReadRecord();
            ++recordCount;
            if (record.Length < schema.TotalWidth)
            {
                hasError = true;
                throw new FlatFileException(Resources.FixedLengthRecordTooShort, recordCount);
            }
            WindowCollection windows = schema.Windows;
            string[] values = new string[windows.Count];
            int offset = 0;
            for (int index = 0; index != values.Length; ++index)
            {
                Window window = windows[index];
                string value = record.Substring(offset, window.Width);
                if (window.Alignment == FixedAlignment.LeftAligned)
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

        /// <summary>
        /// Gets the values for the current record.
        /// </summary>
        /// <returns>The values of the current record.</returns>
        public object[] GetValues()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("FixedLengthReader");
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
