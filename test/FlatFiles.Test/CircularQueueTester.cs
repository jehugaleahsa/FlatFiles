using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FlatFiles.Test
{
    public class CircularQueueTester
    {
        [Fact]
        public void TestInitialCount()
        {
            CircularQueue<int> queue = new CircularQueue<int>();
            Assert.Equal(0, queue.Count);
        }

        [Fact]
        public void TestAdd()
        {
            CircularQueue<int> queue = new CircularQueue<int>();
            queue.Enqueue(1);
            Assert.Equal(1, queue.Count);
        }

        [Fact]
        public void TestAddAndRemove()
        {
            CircularQueue<int> queue = new CircularQueue<int>();

            queue.Enqueue(1);
            Assert.Equal(1, queue.Count);

            var value = queue.Peek();
            queue.Dequeue();
            Assert.Equal(0, queue.Count);
            Assert.Equal(1, value);
        }

        [Fact]
        public void TestShiftedFront()
        {
            CircularQueue<int> queue = new CircularQueue<int>();
            for (int i = 0; i != 8; ++i)
            {
                queue.Enqueue(i);
            }
            queue.Dequeue(2);
            for (int i = 8; i != 14; ++i)
            {
                queue.Enqueue(i);
            }
            var value = queue.Peek();
            queue.Dequeue();
            Assert.Equal(2, value);
        }

        [Fact]
        public void TestLargeCollection()
        {
            const int size = 10000;
            CircularQueue<int> queue = new CircularQueue<int>();
            for (int i = 0; i != size; ++i)
            {
                queue.Enqueue(i);
            }
            Assert.Equal(size, queue.Count);
            while (queue.Count != 0)
            {
                queue.Dequeue();
            }
        }

        [Fact]
        public void TestLargeShifting()
        {
            int size = 2;
            int lastDequeued = -1;
            CircularQueue<int> queue = new CircularQueue<int>();
            for (int iteration = 0, value = 0; iteration != 1000; ++iteration)
            {
                for (int i = 0; i != size; ++i, ++value)
                {
                    queue.Enqueue(value);
                }
                int half = size / 2;
                for (int i = 0; i != half; ++i)
                {
                    lastDequeued = queue.Peek();
                    queue.Dequeue();
                }
                ++size;
            }
        }

        [Fact]
        public void TestLargeRangeShifting()
        {
            int size = 2;
            int value = 0;
            List<int> captured = new List<int>();
            CircularQueue<int> queue = new CircularQueue<int>();
            for (int iteration = 0; iteration != 1000; ++iteration)
            {
                List<int> values = new List<int>(size);
                for (int i = 0; i != size; ++i, ++value)
                {
                    values.Add(value);
                }
                queue.EnqueueRange(values.ToArray(), values.Count);
                int half = size / 2;
                for (int i = 0; i != half; ++i)
                {
                    captured.Add(queue.Peek());
                    queue.Dequeue();
                }
                ++size;
            }
            while (queue.Count != 0)
            {
                captured.Add(queue.Peek());
                queue.Dequeue();
            }
            var expected = Enumerable.Range(0, value).ToArray();
            Assert.Equal(expected, captured);
        }
    }
}
