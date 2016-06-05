using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlatFiles
{
    internal sealed class RetryReader : IDisposable
    {
        private readonly StreamReader reader;
        private readonly bool ownsStream;
        private readonly Stack<char> retry;
        private char current;

        public RetryReader(Stream stream, Encoding encoding, bool ownsStream)
        {
            this.reader = new StreamReader(stream, encoding);
            this.ownsStream = ownsStream;
            this.retry = new Stack<char>();
        }

        ~RetryReader()
        {
            dispose(false);
        }

        public Encoding Encoding
        {
            get
            {
                return reader.CurrentEncoding;
            }
        }

        public bool EndOfStream
        {
            get
            {
                return retry.Count == 0 && reader.EndOfStream;
            }
        }

        public char Current
        {
            get
            {
                return current;
            }
        }

        public bool Read()
        {
            if (retry.Count > 0)
            {
                current = retry.Pop();
                return true;
            }
            if (reader.EndOfStream)
            {
                return false;
            }
            current = (char)reader.Read();
            return true;
        }

        public bool IsMatch(char value)
        {
            if (retry.Count > 0)
            {
                if (retry.Peek() == value)
                {
                    current = retry.Pop();
                    return true;
                }
                return false;
            }
            if (!reader.EndOfStream && reader.Peek() == value)
            {
                current = (char)reader.Read();
                return true;
            }
            return false;
        }

        public bool IsMatch(string value)
        {
            // Optimized for two character separators.
            int position = 0;
            if (!IsMatch(value[position]))
            {
                return false;
            }
            ++position;
            if (position == value.Length)
            {
                return true;
            }
            if (!IsMatch(value[position]))
            {
                Undo(value[0]);
                return false;
            }
            ++position;
            if (position == value.Length)
            {
                return true;
            }
            List<char> tail = new List<char>(value.Length);
            tail.Add(value[0]);
            tail.Add(value[1]);
            while (IsMatch(value[position]))
            {
                tail.Add(current);
                ++position;
                if (position == value.Length)
                {
                    return true;
                }
            }
            Undo(tail);
            return false;
        }

        public void Undo(char item)
        {
            retry.Push(item);
        }

        public void Undo(List<char> items)
        {
            int position = items.Count;
            while (position != 0)
            {
                --position;
                retry.Push(items[position]);
            }
        }

        public void Dispose()
        {
            dispose(true);
        }

        private void dispose(bool isDisposing)
        {
            if (isDisposing && ownsStream)
            {
                reader.Dispose();
            }
        }
    }
}
