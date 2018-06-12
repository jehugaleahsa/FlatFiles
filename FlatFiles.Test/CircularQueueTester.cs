using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class CircularQueueTester
    {
        [TestMethod]
        public void TestInitialCount()
        {
            CircularQueue<int> queue = new CircularQueue<int>(10);
            Assert.AreEqual(0, queue.Count);
        }

        [TestMethod]
        public void TestReserve_Uninitialized_CreatesNewArray()
        {
            CircularQueue<int> queue = new CircularQueue<int>(10);
            var segment = queue.PrepareBlock();

            Assert.AreEqual(0, queue.Count);
            Assert.AreEqual(0, segment.Offset);
            Assert.AreEqual(10, segment.Count);
        }

        [TestMethod]
        public void TestReserve_HasRoom_RoomAtBeginning_ReturnsFront()
        {
            CircularQueue<int> queue = new CircularQueue<int>(10);
            var segment = queue.PrepareBlock();
            fill(segment);
            queue.RecordGrowth(10);
            queue.Dequeue(5);
            segment = queue.PrepareBlock();  // We have room for this, but the elements are at the end. Shift them.

            Assert.AreEqual(5, queue.Count);
            Assert.AreEqual(0, segment.Offset);
            Assert.AreEqual(5, segment.Count);
        }

        [TestMethod]
        public void TestReserve_HasRoom_RoomAtEnd_ReturnsEnd()
        {
            CircularQueue<int> queue = new CircularQueue<int>(10);
            var segment = queue.PrepareBlock();
            fill(segment);
            queue.RecordGrowth(5);  // Claim we only added 5 items
            segment = queue.PrepareBlock();  // We have room for this and there's room at the end.

            Assert.AreEqual(5, segment.Offset);
            Assert.AreEqual(5, segment.Count);
        }

        [TestMethod]
        public void TestReserve_EnoughRoom_SpaceInMiddle_CopyAndShift()
        {
            CircularQueue<int> queue = new CircularQueue<int>(10);
            var segment = queue.PrepareBlock();
            fill(segment);
            queue.RecordGrowth(10);
            queue.Dequeue(8);
            queue.RecordGrowth(4);  // Pretend like we only added 5 items.

            segment = queue.PrepareBlock();
            
            Assert.AreEqual(4, segment.Offset);
            Assert.AreEqual(4, segment.Count);
        }

        [TestMethod]
        public void TestReserve_EnoughRoom_SpaceWrapsAround_CopyAndShift()
        {
            CircularQueue<int> queue = new CircularQueue<int>(10);
            var segment = queue.PrepareBlock();
            fill(segment);
            queue.RecordGrowth(8);
            queue.Dequeue(2);

            segment = queue.PrepareBlock();

            Assert.AreEqual(6, segment.Offset);
            Assert.AreEqual(4, segment.Count);
        }

        private static void fill(ArraySegment<int> segment)
        {
            for (int i = 0; i != segment.Count; ++i)
            {
                segment.Array[i + segment.Offset] = i;
            }
        }
    }
}
