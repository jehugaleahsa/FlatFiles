using System.IO;
using System.Threading.Tasks;

namespace FlatFiles
{
    internal sealed class RetryReader
    {
        private readonly CircularQueue<char> _queue = new CircularQueue<char>(4096);
        private readonly TextReader _reader;
        private bool _isEndOfStreamFound;

        public RetryReader(TextReader reader)
        {
            _reader = reader;
        }

        public bool IsEndOfStream()
        {
            return _isEndOfStreamFound && _queue.Count == 0;
        }

        public char Current { get; private set; }

        public bool Read()
        {
            if (_queue.Count == 0)
            {
                return false;
            }
            Current = _queue.Peek();
            _queue.Dequeue(1);
            return true;
        }

        public bool ShouldLoadBuffer(int minSize)
        {
            return !_isEndOfStreamFound && _queue.Count < minSize;
        }

        public void LoadBuffer()
        {
            if (_isEndOfStreamFound)
            {
                return;
            }
            var segment = _queue.PrepareBlock();
            int length = _reader.ReadBlock(segment.Array, segment.Offset, segment.Count);
            if (length < segment.Count)
            {
                _isEndOfStreamFound = true;
            }
            _queue.RecordGrowth(length);
        }

        public async Task LoadBufferAsync()
        {
            if (_isEndOfStreamFound)
            {
                return;
            }
            var segment = _queue.PrepareBlock();
            int length = await _reader.ReadBlockAsync(segment.Array, segment.Offset, segment.Count).ConfigureAwait(false);
            if (length < segment.Count)
            {
                _isEndOfStreamFound = true;
            }
            _queue.RecordGrowth(length);
        }

        public bool IsWhitespace()
        {
            if (_queue.Count == 0 || !char.IsWhiteSpace(_queue.Peek()))
            {
                return false;
            }
            _queue.Dequeue(1);
            return true;
        }

        public bool IsMatch1(char value)
        {
            if (_queue.Count == 0 || _queue.Peek() != value)
            {
                return false;
            }
            _queue.Dequeue(1);
            return true;
        }

        public bool IsMatch2(char first, char second)
        {
            if (_queue.Count < 2 || _queue.Peek() != first || _queue.Peek(1) != second)
            {
                return false;
            }
            _queue.Dequeue(2);
            return true;
        }

        public bool IsMatch(string value)
        {
            if (_queue.Count < value.Length)
            {
                return false;
            }
            for (int position = 0; position != value.Length; ++position)
            {
                if (_queue.Peek(position) != value[position])
                {
                    return false;
                }
            }
            _queue.Dequeue(value.Length);
            return true;
        }
    }
}
