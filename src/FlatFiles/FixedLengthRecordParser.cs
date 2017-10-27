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
            if (options.HasRecordSeparator)
            {
                this.recordReader = new SeparatorRecordReader(reader, options.RecordSeparator);
            }
            else
            {
                this.recordReader = new FixedLengthRecordReader(reader, schema.TotalWidth);
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

        private sealed class SeparatorRecordReader : IRecordReader
        {
            private readonly RetryReader reader;
            private readonly ISeparatorMatcher matcher;

            public SeparatorRecordReader(TextReader reader, string separator)
            {
                this.reader = new RetryReader(reader);
                this.matcher = SeparatorMatcher.GetMatcher(this.reader, separator);
            }

            public bool EndOfStream
            {
                get { return reader.EndOfStream; }
            }

            public string ReadRecord()
            {
                StringBuilder builder = new StringBuilder();
                while (!matcher.IsMatch() && reader.Read())
                {
                    builder.Append(reader.Current);
                }
                return builder.ToString();
            }
        }

        private sealed class FixedLengthRecordReader : IRecordReader
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
