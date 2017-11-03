using System;

namespace FlatFiles
{
    internal sealed class CircularQueue<T>
    {
        private T[] items;
        private int front;
        private int back;

        public int Count { get; private set; }

        public ArraySegment<T> Reserve(int size)
        {
            if (size == 0)
            {
                return new ArraySegment<T>(items, back, 0);
            }
            if (items == null)
            {
                items = new T[size];
                return new ArraySegment<T>(items, 0, size);
            }
            if (Count + size <= items.Length)
            {
                // The buffer is large enough to hold the new items.
                // Determine if we need to shift the items to create a contiguous block.
                if (front > back || items.Length - back < size)
                {
                    // If the back has wrapped around to the front of the array, we need to shift the elements.
                    // If the back is too close to the end of the array, we need to shift the elements.
                    rotateLeft(items, 0, front, front + Count);
                    front = 0;
                    back = Count;
                }
                return new ArraySegment<T>(items, back, size);
            }
            else
            {
                // The buffer is not large enough to hold the new items.
                // Create a new buffer, copying the current contents into it, shifting the content to the front.
                T[] newItems = new T[Math.Max(Count + size, items.Length * 2)];
                if (back < front)
                {
                    // The back has wrapped around to the front of the array; copy from the front to the end (the middle) of the old list.
                    // Then copy from the front of the old list (the middle) until the end.
                    int length = items.Length - front;
                    Array.Copy(items, front, newItems, 0, length);
                    Array.Copy(items, 0, newItems, length, back);
                }
                else
                {
                    // The range, from the front to the back, is contiguous. Copy to the new buffer and shift the elements to the front.
                    int length = back - front;
                    Array.Copy(items, front, newItems, 0, length);
                }
                front = 0;
                back = Count;
                items = newItems;
                return new ArraySegment<T>(items, Count, size);
            }
        }

        public void AddItemCount(int size)
        {
            Count += size;
            back += size;
            if (back >= items.Length)
            {
                back -= items.Length;
            }
        }

        private static void rotateLeft(T[] list, int first, int middle, int past)
        {
            int shift = middle - first;
            int count = past - first;
            for (int factor = shift; factor != 0;)
            {
                int temp = count % factor;
                count = factor;
                factor = temp;
            }
            if (count < past - first)
            {
                while (count > 0)
                {
                    int hole = first + count;
                    T value = list[hole];
                    int temp = hole + shift;
                    int next = temp == past ? first : temp;
                    int current = hole;
                    while (next != hole)
                    {
                        list[current] = list[next];
                        current = next;
                        int difference = past - next;
                        if (shift < difference)
                        {
                            next += shift;
                        }
                        else
                        {
                            next = first + (shift - difference);
                        }
                    }
                    list[current] = value;
                    --count;
                }
            }
        }

        public T Peek()
        {
            return items[front];
        }

        public T Peek(int index)
        {
            int position = front + index;
            if (position >= items.Length)
            {
                position -= items.Length;
            }
            return items[position];
        }

        public void Dequeue(int count)
        {
            front += count;
            if (front >= items.Length)
            {
                front -= items.Length;
            }
            Count -= count;
        }
    }
}
