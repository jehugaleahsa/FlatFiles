using System;

namespace FlatFiles
{
    internal sealed class CircularQueue<T>
    {
        private readonly T[] _items;
        private int _front;
        private int _back;

        public CircularQueue(int bufferSize)
        {
            _items = new T[bufferSize];
        }

        public int Count { get; private set; }

        public ArraySegment<T> PrepareBlock()
        {
            int size = _items.Length - Count;
            // The buffer is large enough to hold the new items.
            // Determine if we need to shift the items to create a contiguous block.
            if (_back >= _front && _items.Length - _back < size)
            {
                Array.Copy(_items, _front, _items, 0, Count);
                _front = 0;
                _back = Count;
            }
            return new ArraySegment<T>(_items, _back, size);
        }

        public void RecordGrowth(int size)
        {
            Count += size;
            _back += size;
            if (_back >= _items.Length)
            {
                _back -= _items.Length;
            }
        }

        public T Peek()
        {
            return _items[_front];
        }

        public T Peek(int index)
        {
            int position = _front + index;
            if (position >= _items.Length)
            {
                position -= _items.Length;
            }
            return _items[position];
        }

        public void Dequeue(int count)
        {
            _front += count;
            if (_front >= _items.Length)
            {
                _front -= _items.Length;
            }
            Count -= count;
        }
    }
}
