using System;
using System.IO;
using System.Threading.Tasks;

namespace FlatFiles
{
    internal sealed class RetryReader
    {
        private const int bufferSize = 4096;
        private readonly TextReader reader;
        private readonly ReaderState readState;
        private readonly RetryState retryState;
        private IReaderState state;
        private char current;

        public RetryReader(TextReader reader)
        {
            this.reader = reader;
            this.readState = new ReaderState(this);
            this.retryState = new RetryState(this);
            this.state = readState;
        }

        public bool IsEndOfStream()
        {
            return state.IsEndOfStream();
        }

        public char Current
        {
            get { return current; }
        }

        public bool Read()
        {
            return state.Read();
        }

        public bool IsBufferLargeEnough(int minSize)
        {
            return retryState.IsBufferLargeEnough(minSize);
        }

        public async Task LoadBuffer(int minSize)
        {
            char[] buffer = new char[bufferSize];
            int length = await reader.ReadBlockAsync(buffer, 0, bufferSize);
            if (length < minSize)
            {
                retryState.IsEndOfStreamFound = true;
            }
            retryState.AddRange(buffer, length);
            state = retryState;
        }

        public bool IsWhitespace()
        {
            int next = state.Peek();
            if (next != -1 && Char.IsWhiteSpace(unchecked((char)next)))
            {
                state.SafeRead();
                return true;
            }
            return false;
        }

        public bool IsMatch1(char value)
        {
            int next = state.Peek();
            if (next != -1 && unchecked((char)next) == value)
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
            retryState.Add(item);
            state = retryState;
        }

        private void undo(char[] items, int length)
        {
            if (length == 0)
            {
                return;
            }
            retryState.AddRange(items, length);
            state = retryState;
        }

        private interface IReaderState
        {
            bool Read();

            void SafeRead();

            int Peek();

            bool IsEndOfStream();
        }

        private sealed class ReaderState : IReaderState
        {
            private const int bufferSize = 4096;
            private readonly RetryReader reader;
            private readonly TextReader textReader;

            public ReaderState(RetryReader reader)
            {
                this.reader = reader;
                this.textReader = reader.reader;
            }

            public bool IsEndOfStream()
            {
                return textReader.Peek() == -1;
            }

            public int Peek()
            {
                return textReader.Peek();
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

            private int read()
            {
                return textReader.Read();
            }
        }

        private sealed class RetryState : IReaderState
        {
            private readonly RetryReader reader;
            private readonly CircularQueue<char> queue;

            public RetryState(RetryReader reader)
            {
                this.reader = reader;
                this.queue = new CircularQueue<char>();
            }

            public bool IsEndOfStreamFound { get; set; }

            public bool IsEndOfStream()
            {
                return IsEndOfStreamFound && queue.Count == 0;
            }

            public bool IsBufferLargeEnough(int minSize)
            {
                return queue.Count >= minSize;
            }

            public void Add(char item)
            {
                queue.Enqueue(item);
            }

            public void AddRange(char[] items, int length)
            {
                queue.EnqueueRange(items, length);
            }

            public int Peek()
            {
                return queue.Count == 0 ? -1 : queue.Peek();
            }

            public bool Read()
            {
                SafeRead();
                return true;
            }

            public void SafeRead()
            {
                reader.current = queue.Dequeue();
                if (queue.Count == 0)
                {
                    reader.state = reader.readState;
                }
            }
        }
    }
}
