using System;
using System.IO;
using System.IO.Compression;
using GZipLibrary.Blocks;
using GZipLibrary.Blocks.Readers;
using GZipLibrary.Blocks.Writers;

namespace GZipLibrary.Processors
{
    public class DecompressionProcessor : BaseProcessor
    {
        private long _lastBlockSize, _blocksCount;

        public DecompressionProcessor(string inputFilePath, string outputFilePath, int queueSize) : base(inputFilePath, outputFilePath, queueSize)
        {
        }

        protected override BlockReader GetBlockReader()
        {
            var fileStream = new FileStream(InputFilePath, FileMode.Open, FileAccess.Read);
            
            FullFileSize = ReadNumberFromStream(fileStream);
            BlockSize = ReadNumberFromStream(fileStream);
            _lastBlockSize = FullFileSize % BlockSize;
            _blocksCount = FullFileSize / BlockSize + (_lastBlockSize == 0 ? 0 : 1);

            return new CompressedBlockReader(fileStream);
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
                    if (block.Id == _blocksCount - 1)
                    {
                        blockSize = _lastBlockSize;
                    }
                    byte[] buffer = new byte[blockSize];
                    gZipStream.Read(buffer, 0, buffer.Length);
                    block.Data = buffer;
                }                
            }
        }

        private long ReadNumberFromStream(Stream stream)
        {
            var buffer = new byte[sizeof(long)];
            stream.Read(buffer, 0, buffer.Length);
            var size = BitConverter.ToInt64(buffer, 0);

            return size;
        }
    }
}
