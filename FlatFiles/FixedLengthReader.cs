using System;
using FlatFiles.Properties;
using System.IO;
using System.Text;

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
            parser = new FixedLengthRecordParser(reader, options);
            this.schema = schema;
            this.options = options.Clone();
            if (this.options.IsFirstRecordHeader)
            {
                skip();
            }
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
        /// Reads the next record from the file.
        /// </summary>
        /// <returns>True if the next record was parsed; otherwise, false if all files are read.</returns>
        public bool Read()
        {
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
            values = schema.ParseValues(rawValues);
            return true;
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
