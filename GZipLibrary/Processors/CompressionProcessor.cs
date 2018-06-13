using System.IO;
using System.IO.Compression;
using GZipLibrary.Blocks;
using GZipLibrary.Blocks.Readers;
using GZipLibrary.Blocks.Writers;

namespace GZipLibrary.Processors
{
    public class CompressionProcessor : BaseProcessor
    {
        public CompressionProcessor(string inputFilePath, string outputFilePath, int blockSize, int queueSize) : base(inputFilePath, outputFilePath, blockSize, queueSize)
        {
        }

        protected override BlockReader GetBlockReader()
        {
            var fileStream = new FileStream(InputFilePath, FileMode.Open, FileAccess.Read);
            return new UncompressedBlockReader(fileStream, BlockSize);
        }

        protected override BlockWriter GetBlockWriter()
        {
            var fileStream = new FileStream(OutputFilePath, FileMode.Create, FileAccess.Write);
            return new CompressedBlockWriter(fileStream, BlockSize);
        }

        protected override void DoActionWithBlock(Block block)
        {
            using (var stream = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(stream, CompressionMode.Compress))
                {
                    gZipStream.Write(block.Data, 0, (int)block.Size);
                }

                block.Data = stream.ToArray();
            }
        }
    }
}
