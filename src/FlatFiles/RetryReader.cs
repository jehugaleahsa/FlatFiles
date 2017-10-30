using System;
using System.IO;
using System.Threading.Tasks;

namespace FlatFiles
{
    internal sealed class RetryReader
    {
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

        public async ValueTask<bool> IsEndOfStreamAsync()
        {
            return await state.IsEndOfStreamAsync();
        }

        public char Current
        {
            get { return current; }
        }

        public bool Read()
        {
            return state.Read();
        }

        public async ValueTask<bool> ReadAsync()
        {
            return await state.ReadAsync();
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

        public async ValueTask<bool> IsWhitespaceAsync()
        {
            int next = await state.PeekAsync();
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

        public async ValueTask<bool> IsMatch1Async(char value)
        {
            int next = await state.PeekAsync();
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

        public async ValueTask<bool> IsMatch2Async(char first, char second)
        {
            if (!await IsMatch1Async(first))
            {
                return false;
            }
            if (!await IsMatch1Async(second))
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

        public async ValueTask<bool> IsMatchAsync(string value)
        {
            int position = 0;
            char[] buffer = new char[value.Length];
            while (await IsMatch1Async(value[position]))
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

            ValueTask<bool> ReadAsync();

            void SafeRead();

            int Peek();

            ValueTask<int> PeekAsync();

            bool IsEndOfStream();

            ValueTask<bool> IsEndOfStreamAsync();
        }

        private sealed class ReaderState : IReaderState
        {
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

            public async ValueTask<bool> IsEndOfStreamAsync()
            {
                return await PeekAsync() == -1;
            }

            public int Peek()
            {
                return textReader.Peek();
            }

            public async ValueTask<int> PeekAsync()
            {
                char[] buffer = new char[4096];
                int length = await textReader.ReadBlockAsync(buffer, 0, buffer.Length);
                if (length == 0)
                {
                    return -1;
                }
                reader.retryState.AddRange(buffer, length);
                reader.state = reader.retryState;
                return await reader.retryState.PeekAsync();
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

            public async ValueTask<bool> ReadAsync()
            {
                char[] buffer = new char[4096];
                int length = await textReader.ReadBlockAsync(buffer, 0, buffer.Length);
                if (length == 0)
                {
                    return false;
                }
                reader.retryState.AddRange(buffer, length);
                reader.state = reader.retryState;
                return await reader.retryState.ReadAsync();
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
            private CircularQueue<char> queue;

            public RetryState(RetryReader reader)
            {
                this.reader = reader;
                this.queue = new CircularQueue<char>();
            }

            public bool IsEndOfStream()
            {
                return false;
            }

            public ValueTask<bool> IsEndOfStreamAsync()
            {
                return new ValueTask<bool>(false);
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
                return queue.Peek();
            }

            public ValueTask<int> PeekAsync()
            {
                return new ValueTask<int>((int)queue.Peek());
            }

            public bool Read()
            {
                SafeRead();
                return true;
            }

            public ValueTask<bool> ReadAsync()
            {
                SafeRead();
                return new ValueTask<bool>(true);
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
