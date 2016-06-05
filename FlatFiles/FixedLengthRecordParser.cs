using System;
using System.IO;
using System.Text;

namespace FlatFiles
{
    internal sealed class FixedLengthRecordParser : IDisposable
    {
        private readonly StreamReader reader;
        private readonly bool ownsStream;
        private readonly string separator;
        private bool isDisposed;

        public FixedLengthRecordParser(Stream stream, FixedLengthOptions options, bool ownsStream)
        {
            this.reader = new StreamReader(stream, options.Encoding ?? new UTF8Encoding(false));
            this.separator = options.RecordSeparator;
            this.ownsStream = ownsStream;
        }

        ~FixedLengthRecordParser()
        {
            dispose(false);
        }

        public Encoding Encoding
        {
            get { return reader.CurrentEncoding; }
        }

        public bool EndOfStream
        {
            get { return reader.EndOfStream; }
        }

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

        public string ReadRecord()
        {
            StringBuilder builder = new StringBuilder();
            int positionIndex = 0;
            while (!reader.EndOfStream && positionIndex != separator.Length)
            {
                int value = reader.Read();
                if (value != -1)
                {
                    char next = (char)value;
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
            }
            if (positionIndex == separator.Length)
            {
                builder.Length -= separator.Length;
            }
            return builder.ToString();
        }
    }
}
