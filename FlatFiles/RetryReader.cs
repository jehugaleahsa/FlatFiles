using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FlatFiles
{
    internal sealed class RetryReader
    {
        private readonly StringBuilder record = new();
        private readonly CircularQueue<char> queue = new(4096);
        private readonly TextReader reader;
        private bool isEndOfStreamFound;

        public RetryReader(TextReader reader)
        {
            this.reader = reader;
        }

        public bool IsEndOfStream()
        {
            return isEndOfStreamFound && queue.Count == 0;
        }

        public char Current { get; private set; }

        public bool Read()
        {
            if (queue.Count == 0)
            {
                return false;
            }
            char current = queue.Peek();
            Current = current;
            queue.Dequeue(1);
            record.Append(current);
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
            var segment = queue.PrepareBlock();
            int length = reader.ReadBlock(segment.Array!, segment.Offset, segment.Count);
            if (length < segment.Count)
            {
                isEndOfStreamFound = true;
            }
            queue.RecordGrowth(length);
        }

        public async Task LoadBufferAsync()
        {
            if (isEndOfStreamFound)
            {
                return;
            }
            var segment = queue.PrepareBlock();
            int length = await reader.ReadBlockAsync(segment.Array!, segment.Offset, segment.Count).ConfigureAwait(false);
            if (length < segment.Count)
            {
                isEndOfStreamFound = true;
            }
            queue.RecordGrowth(length);
        }

        public bool IsWhitespace()
        {
            if (queue.Count == 0 || !char.IsWhiteSpace(queue.Peek()))
            {
                return false;
            }
            record.Append(queue.Peek());
            queue.Dequeue(1);
            return true;
        }

        public bool IsMatch1(char value)
        {
            if (queue.Count == 0 || queue.Peek() != value)
            {
                return false;
            }
            record.Append(value);
            queue.Dequeue(1);
            return true;
        }

        public bool IsMatch2(char first, char second)
        {
            if (queue.Count < 2 || queue.Peek() != first || queue.Peek(1) != second)
            {
                return false;
            }
            record.Append(first);
            record.Append(second);
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
            record.Append(value);
            queue.Dequeue(value.Length);
            return true;
        }

        public string GetRecord()
        {
            string record = this.record.ToString();
            this.record.Clear();
            return record;
        }
    }
}
