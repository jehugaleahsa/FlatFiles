using System;
using System.IO;
using System.Text;

namespace FlatFiles
{
    internal sealed class FixedLengthRecordParser
    {
        private readonly TextReader reader;
        private readonly string separator;

        public FixedLengthRecordParser(TextReader reader, FixedLengthOptions options)
        {
            this.reader = reader;
            this.separator = options.RecordSeparator;
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
}
