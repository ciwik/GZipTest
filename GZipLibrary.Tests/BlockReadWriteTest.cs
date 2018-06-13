using System.IO;
using GZipLibrary.Blocks;
using GZipLibrary.Blocks.Readers;
using GZipLibrary.Blocks.Writers;
using NUnit.Framework;

namespace GZipLibrary.Tests
{
    [TestFixture]
    public class BlockReadWriteTest
    {
        [Test]
        public void ReadUncompressedBlockFromStream()
        {
            //Arrange
            byte[] data = {1, 1, 2, 3, 5, 8, 13};
            var stream = new MemoryStream(data);
            var reader = new UncompressedBlockReader(stream, data.Length);

            //Act
            var result = reader.Read(out Block newBlock);

            //Assert
            Assert.True(result);
            Assert.AreEqual(data, newBlock.Data);
        }

        [Test]
        public void WriteUncompressedBlockToStream()
        {
            //Arrange
            byte[] data = { 1, 1, 2, 3, 5, 8, 13 };
            Block block = new Block(0, data);
            var stream = new MemoryStream();
            var writer = new UncompressedBlockWriter(stream, data.Length);

            //Act
            writer.Write(block);
            byte[] newData = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(newData, 0, newData.Length);

            //Assert
            Assert.AreEqual(data, newData);
        }

        [Test]
        public void ReadWriteBlock()
        {
            //Arrange
            byte[] data = { 1, 1, 2, 3, 5, 8, 13 };            
            var inputStream = new MemoryStream(data);
            var outputStream = new MemoryStream();
            BlockReader reader = new UncompressedBlockReader(inputStream, data.Length);
            BlockWriter writer = new CompressedBlockWriter(outputStream);

            //Act
            var result1 = reader.Read(out Block block1);
            writer.Write(block1);

            outputStream.Seek(0, SeekOrigin.Begin);
            reader = new CompressedBlockReader(outputStream);            
            var result2 = reader.Read(out Block block2);

            //Assert
            Assert.True(result1 && result2);
            Assert.AreEqual(data, block2.Data);
        }
    }
}
