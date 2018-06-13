using GZipLibrary.Blocks;
using NUnit.Framework;

namespace GZipTest.Tests
{
    [TestFixture]
    public class BlockQueueTest
    {
        [Test]
        public void EnqueueDequeueOneBlock()
        {
            //Arrange
            int queueLength = 3;
            var queue = new BlockQueue(queueLength);
            Block inputBlock = new Block(1, new byte[] { 1, 1, 2, 3, 5, 8 });
            Block outputBlock;          

            //Act
            queue.Enqueue(inputBlock);            
            bool result = queue.TryDequeue(out outputBlock);

            //Assert
            Assert.True(result);
            Assert.AreEqual(inputBlock, outputBlock);
        }

        [Test]
        public void EnqueueDequeueManyBlocks()
        {
            //Arrange
            int queueLength = 3;
            var queue = new BlockQueue(queueLength);
            Block inputBlock1 = new Block(1, new byte[] {1, 1, 2, 3, 5, 8});
            Block inputBlock2 = new Block(2, new byte[] {13, 21});
            Block inputBlock3 = new Block(3, new byte[] {34, 55, 89});

            //Act
            queue.Enqueue(inputBlock1);
            queue.Enqueue(inputBlock2);
            queue.Enqueue(inputBlock3);

            bool result1 = queue.TryDequeue(out var outputBlock1);
            bool result2 = queue.TryDequeue(out var outputBlock2);
            bool result3 = queue.TryDequeue(out var outputBlock3);

            //Assert
            Assert.True(result1);
            Assert.True(result2);
            Assert.True(result3);

            Assert.AreEqual(inputBlock1, outputBlock1);
            Assert.AreEqual(inputBlock2, outputBlock2);
            Assert.AreEqual(inputBlock3, outputBlock3);
        }
    }
}
