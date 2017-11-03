using System;
using System.IO;
using System.Threading.Tasks;

namespace FlatFiles
{
    internal sealed class RetryReader
    {
        private const int bufferSize = 4096;
        private readonly TextReader reader;
        private readonly CircularQueue<char> queue;
        private char current;
        private bool isEndOfStreamFound;

        public RetryReader(TextReader reader)
        {
            this.reader = reader;
            this.queue = new CircularQueue<char>();
        }

        public bool IsEndOfStream()
        {
            return isEndOfStreamFound && queue.Count == 0;
        }

        public char Current
        {
            get { return current; }
        }

        public bool Read()
        {
            if (queue.Count == 0)
            {
                return false;
            }
            current = queue.Peek();
            queue.Dequeue(1);
            return true;
        }

        public bool ShouldLoadBuffer(int minSize)
        {
            return !isEndOfStreamFound && queue.Count < minSize;
        }

        public void LoadBuffer()
        {
            if (isEndOfStreamFound)
            {
                return;
            }
            var segment = queue.Reserve(bufferSize);
            int length = reader.ReadBlock(segment.Array, segment.Offset, segment.Count);
            if (length < bufferSize)
            {
                isEndOfStreamFound = true;
            }
            queue.AddItemCount(length);
        }

        public async Task LoadBufferAsync()
        {
            if (isEndOfStreamFound)
            {
                return;
            }
            var segment = queue.Reserve(bufferSize);
            int length = await reader.ReadBlockAsync(segment.Array, segment.Offset, segment.Count);
            if (length < bufferSize)
            {
                isEndOfStreamFound = true;
            }
            queue.AddItemCount(length);
        }

        public bool IsWhitespace()
        {
            if (queue.Count == 0 || !Char.IsWhiteSpace(queue.Peek()))
            {
                return false;
            }
            queue.Dequeue(1);
            return true;
        }

        public bool IsMatch1(char value)
        {
            if (queue.Count == 0 || queue.Peek() != value)
            {
                return false;
            }
            queue.Dequeue(1);
            return true;
        }

        public bool IsMatch2(char first, char second)
        {
            if (queue.Count < 2 || queue.Peek() != first || queue.Peek(1) != second)
            {
                return false;
            }
            queue.Dequeue(2);
            return true;
        }

        public bool IsMatch(string value)
        {
            if (queue.Count < value.Length)
            {
                return false;
            }
            for (int position = 0; position != value.Length; ++position)
            {
                if (queue.Peek(position) != value[position])
                {
                    return false;
                }
            }
            queue.Dequeue(value.Length);
            return true;
        }
    }
}
