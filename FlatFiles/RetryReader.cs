using System;
using System.Collections.Generic;
using System.IO;

namespace FlatFiles
{
    internal sealed class RetryReader
    {
        private readonly TextReader reader;
        private readonly Stack<char> retry;
        private readonly ReaderState readState;
        private readonly RetryState retryState;
        private IReaderState state;
        private char current;

        public RetryReader(TextReader reader)
        {
            this.reader = reader;
            this.retry = new Stack<char>();
            this.readState = new ReaderState(this);
            this.retryState = new RetryState(this);
            this.state = readState;
        }

        public bool EndOfStream
        {
            get { return state.EndOfStream; }
        }

        public char Current
        {
            get { return current; }
        }

        public bool Read()
        {
            return state.Read();
        }

        public bool IsMatch(Func<char, bool> comparer)
        {
            int next = state.Peek();
            if (next != -1 && comparer(unchecked((char)next)))
            {
                state.SafeRead();
                return true;
            }
            return false;
        }

        public bool IsMatch1(char value)
        {
            int next = state.Peek();
            if (next == -1)
            {
                return false;
            }
            if (unchecked((char)next) == value)
            {
                state.SafeRead();
                return true;
            }
            return false;
        }

        public bool IsMatch2(char first, char second)
        {
            if (!IsMatch1(first))
            {
                return false;
            }
            if (!IsMatch1(second))
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
            while (IsMatch1(value[position]))
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
            state = retryState;
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
            state = retryState;
        }

        private interface IReaderState
        {
            bool Read();

            void SafeRead();

            int Peek();

            bool EndOfStream { get; }
        }

        private sealed class ReaderState : IReaderState
        {
            private readonly RetryReader reader;
            private readonly TextReader textReader;
            private int? peekValue;

            public ReaderState(RetryReader reader)
            {
                this.reader = reader;
                this.textReader = reader.reader;
            }

            private int getPeeked()
            {
                if (!peekValue.HasValue)
                {
                    peekValue = textReader.Peek();
                }                
                return peekValue.Value;
            }

            private int read()
            {
                peekValue = null;
                return textReader.Read();
            }

            public bool EndOfStream
            {
                get { return getPeeked() == -1; }
            }

            public int Peek()
            {
                return getPeeked();
            }

            public bool Read()
            {
                int next = read();
                if (next == -1)
                {
                    return false;
                }
                reader.current = unchecked((char)next);
                return true;
            }

            public void SafeRead()
            {
                reader.current = unchecked((char)read());
            }
        }

        private sealed class RetryState : IReaderState
        {
            private readonly RetryReader reader;
            private readonly Stack<char> retry;

            public RetryState(RetryReader reader)
            {
                this.reader = reader;
                this.retry = reader.retry;
            }

            public bool EndOfStream
            {
                get { return false; }
            }

            public int Peek()
            {
                return retry.Peek();
            }

            public bool Read()
            {
                SafeRead();
                return true;
            }

            public void SafeRead()
            {
                reader.current = retry.Pop();
                if (retry.Count == 0)
                {
                    reader.state = reader.readState;
                }
            }
        }
    }
}
