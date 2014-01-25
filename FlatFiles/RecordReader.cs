using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlatFiles
{
    internal sealed class RecordReader : IDisposable
    {
        private readonly StreamReader reader;
        private readonly bool ownsStream;
        private readonly string separator;
        private bool isDisposed;

        public RecordReader(Stream stream, Encoding encoding, string separator, bool ownsStream)
        {
            this.reader = new StreamReader(new BufferedStream(stream), encoding ?? Encoding.Default);
            this.separator = separator;
            this.ownsStream = ownsStream;
        }

        ~RecordReader()
        {
            dispose(false);
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
            List<char> buffer = new List<char>();
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
                    buffer.Add(next);
                }
            }
            if (positionIndex == separator.Length)
            {
                buffer.RemoveRange(buffer.Count - separator.Length, separator.Length);
            }
            return new String(buffer.ToArray());
        }
    }
}
