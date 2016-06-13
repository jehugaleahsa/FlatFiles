using System;
using System.Collections.Generic;
using System.IO;

namespace FlatFiles
{
    internal sealed class RetryReader
    {
        private readonly TextReader reader;
        private readonly Stack<char> retry;
        private Func<bool> read;
        private Action safeRead;
        private Func<int> peek;
        private Func<bool> eos;
        private char current;

        public RetryReader(TextReader reader)
        {
            this.reader = reader;
            this.retry = new Stack<char>();
            this.read = readerRead;
            this.safeRead = safeReaderRead;
            this.peek = readerPeek;
            this.eos = readerEos;
        }

        public bool EndOfStream
        {
            get { return eos(); }
        }

        public char Current
        {
            get { return current; }
        }

        public bool Read()
        {
            return read();
        }

        private bool readerRead()
        {
            int next = reader.Read();
            if (next == -1)
            {
                return false;
            }
            current = (char)next;
            return true;
        }

        private bool retryRead()
        {
            safeRetryRead();
            return true;
        }

        private void safeReaderRead()
        {
            int next = reader.Read();
            current = (char)next;
        }

        private void safeRetryRead()
        {
            current = retry.Pop();
            if (retry.Count == 0)
            {
                read = readerRead;
                safeRead = safeReaderRead;
                peek = readerPeek;
                eos = readerEos;
            }
        }

        private int readerPeek()
        {
            return reader.Peek();
        }

        private int retryPeek()
        {
            return retry.Peek();
        }

        private bool readerEos()
        {
            return reader.Peek() == -1;
        }

        private bool retryEos()
        {
            return false;
        }

        public bool IsMatch(Func<char, bool> comparer)
        {
            int next = peek();
            if (next != -1 && comparer((char)next))
            {
                safeRead();
                return true;
            }
            return false;
        }

        public bool IsMatch(char value)
        {
            int next = peek();
            if (next != -1 && (char)next == value)
            {
                safeRead();
                return true;
            }
            return false;
        }

        public bool IsMatch1(string value)
        {
            return IsMatch(value[0]);
        }

        public bool IsMatch2(string value)
        {
            if (!IsMatch(value[0]))
            {
                return false;
            }
            if (!IsMatch(value[1]))
            {
                undo(current);
                return false;
            }
            return true;
        }

        public bool IsMatch(string value)
        {
            int position = 0;
            char[] buffer = new char[value.Length];
            while (IsMatch(value[position]))
            {
                buffer[position] = value[position];
                ++position;
                if (position == value.Length)
                {
                    return true;
                }
            }
            undo(buffer, position);
            return false;
        }

        private void undo(char item)
        {
            retry.Push(item);
            read = retryRead;
            safeRead = safeRetryRead;
            peek = retryPeek;
            eos = retryEos;
        }

        private void undo(char[] items, int length)
        {
            if (length == 0)
            {
                return;
            }
            do
            {
                --length;
                retry.Push(items[length]);
            }
            while (length != 0);
            read = retryRead;
            safeRead = safeRetryRead;
            peek = retryPeek;
            eos = retryEos;
        }
    }
}
