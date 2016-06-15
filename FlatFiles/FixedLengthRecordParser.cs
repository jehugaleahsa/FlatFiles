using System;
using System.IO;
using System.Text;

namespace FlatFiles
{
    internal sealed class FixedLengthRecordParser
    {
        private readonly IRecordReader recordReader;

        public FixedLengthRecordParser(TextReader reader, FixedLengthSchema schema, FixedLengthOptions options)
        {
            if (String.IsNullOrEmpty(options.RecordSeparator))
            {
                this.recordReader = new FixedLengthRecordReader(reader, schema.TotalWidth);
            }
            else
            {
                this.recordReader = new SeparatorRecordReader(reader, options.RecordSeparator);
            }
        }

        public bool EndOfStream
        {
            get { return recordReader.EndOfStream; }
        }

        public string ReadRecord()
        {
            return recordReader.ReadRecord();
        }

        private interface IRecordReader
        {
            bool EndOfStream { get; }

            string ReadRecord();
        }

        private class SeparatorRecordReader : IRecordReader
        {
            private readonly TextReader reader;
            private readonly string separator;

            public SeparatorRecordReader(TextReader reader, string separator)
            {
                this.reader = reader;
                this.separator = separator;
            }

            public bool EndOfStream
            {
                get { return reader.Peek() == -1; }
            }

            public string ReadRecord()
            {
                StringBuilder builder = new StringBuilder();
                int positionIndex = 0;
                while (reader.Peek() != -1 && positionIndex != separator.Length)
                {
                    char next = (char)reader.Read();
                    if (next == separator[positionIndex])
                    {
                        ++positionIndex;
                    }
                    else
                    {
                        positionIndex = 0;
                    }
                    builder.Append(next);
                }
                if (positionIndex == separator.Length)
                {
                    builder.Length -= separator.Length;
                }
                return builder.ToString();
            }
        }

        private class FixedLengthRecordReader : IRecordReader
        {
            private readonly TextReader reader;
            private readonly char[] buffer;

            public FixedLengthRecordReader(TextReader reader, int totalWidth)
            {
                this.reader = reader;
                this.buffer = new char[totalWidth];
            }

            public bool EndOfStream
            {
                get { return reader.Peek() == -1; }
            }

            public string ReadRecord()
            {
                int length = reader.ReadBlock(buffer, 0, buffer.Length);
                return new String(buffer, 0, length);
            }
        }
    }
}
