using System.IO;
using System.IO.Compression;
using GZipLibrary.Blocks;
using GZipLibrary.Blocks.Readers;
using GZipLibrary.Blocks.Writers;

namespace GZipLibrary.Processors
{
    public class DecompressionProcessor : BaseProcessor
    {
        public DecompressionProcessor(string inputFilePath, string outputFilePath, int blockSize, int queueSize) : base(inputFilePath, outputFilePath, blockSize, queueSize)
        {
        }

        protected override BlockReader GetBlockReader()
        {
            var fileStream = new FileStream(InputFilePath, FileMode.Open, FileAccess.Read);
            return new CompressedBlockReader(fileStream, BlockSize + sizeof(long));
        }

        protected override BlockWriter GetBlockWriter()
        {
            var fileStream = new FileStream(OutputFilePath, FileMode.Create, FileAccess.Write);
            return new UncompressedBlockWriter(fileStream, BlockSize);
        }

        protected override void DoActionWithBlock(Block block)
        {
            using (var stream = new MemoryStream(block.Data))
            {
                using (var gZipStream = new GZipStream(stream, CompressionMode.Decompress))
                {
                    var blockSize = BlockSize;
                    if (block.Id == BlocksCount - 1)
                    {
                        blockSize = (int)LastBlockSize;
                    }
                    byte[] buffer = new byte[blockSize];
                    gZipStream.Read(buffer, 0, buffer.Length);
                    block.Data = buffer;
                }                
            }
        }
    }
}
