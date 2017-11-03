using System;
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
        public void TestReserve_Uninitialized_CreatesNewArray()
        {
            CircularQueue<int> queue = new CircularQueue<int>();
            var segment = queue.Reserve(10);

            Assert.Equal(0, queue.Count);
            Assert.Equal(0, segment.Offset);
            Assert.Equal(10, segment.Count);
        }

        [Fact]
        public void TestReserve_NoRoom_ExpandsBuffer()
        {
            CircularQueue<int> queue = new CircularQueue<int>();
            queue.Reserve(1);

            var segment = queue.Reserve(10);

            Assert.Equal(0, queue.Count);
            Assert.Equal(0, segment.Offset);
            Assert.Equal(10, segment.Count);
        }

        [Fact]
        public void TestReserve_HasRoom_NoRoomAtEnd_ShiftsBuffer()
        {
            CircularQueue<int> queue = new CircularQueue<int>();
            var segment = queue.Reserve(10);
            segment = fill(segment);
            queue.AddItemCount(10);
            dequeue(queue, 5);
            segment = queue.Reserve(5);  // We have room for this, but the elements are at the end. Shift them.

            Assert.Equal(5, queue.Count);
            Assert.Equal(5, segment.Offset);
            Assert.Equal(5, segment.Count);
        }

        [Fact]
        public void TestReserve_HasRoom_RoomAtEnd_ReturnsEnd()
        {
            CircularQueue<int> queue = new CircularQueue<int>();
            var segment = queue.Reserve(10);
            segment = fill(segment);
            queue.AddItemCount(5);  // Claim we only added 5 items
            segment = queue.Reserve(5);  // We have room for this and there's room at the end.

            Assert.Equal(5, segment.Offset);
            Assert.Equal(5, segment.Count);
        }

        [Fact]
        public void TestReserve_NotEnoughRoom_ItemsInMiddle_CopiesAndShifts()
        {
            CircularQueue<int> queue = new CircularQueue<int>();
            var segment = queue.Reserve(10);
            segment = fill(segment);
            queue.AddItemCount(10);
            dequeue(queue, 5);  // Remove the front 5 items
            segment = queue.Reserve(10);  // We are asking for 10, but only have room for 5.

            Assert.Equal(5, queue.Count);
            Assert.Equal(5, segment.Offset);
            Assert.Equal(10, segment.Count);
        }

        [Fact]
        public void TestReserve_NotEnoughRoom_ItemsAtFront_CopiesAndShifts()
        {
            CircularQueue<int> queue = new CircularQueue<int>();
            var segment = queue.Reserve(10);
            segment = fill(segment);
            queue.AddItemCount(5);  // Pretend like we only added 5 items.
            segment = queue.Reserve(10);  // We are asking for 10, but only have room for 5.

            Assert.Equal(5, queue.Count);
            Assert.Equal(5, segment.Offset);
            Assert.Equal(10, segment.Count);
        }

        private static ArraySegment<int> fill(ArraySegment<int> segment)
        {
            for (int i = 0; i != segment.Count; ++i)
            {
                segment.Array[i + segment.Offset] = i;
            }
            return segment;
        }

        private static void dequeue(CircularQueue<int> queue, int count)
        {
            for (int i = 0; i != count; ++i)
            {
                queue.Dequeue(1);
            }
        }
    }
}
