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

        public DecompressionProcessor(Stream inputStream, Stream outputStream, int queueSize) : base(inputStream, outputStream, queueSize)
        {
        }

        protected override BlockReader GetBlockReader()
        {
            FullUncompressedStreamLength = ReadNumberFromStream(InputStream);
            BlockSize = ReadNumberFromStream(InputStream);
            _lastBlockSize = FullUncompressedStreamLength % BlockSize;
            _blocksCount = FullUncompressedStreamLength / BlockSize + (_lastBlockSize == 0 ? 0 : 1);

            return new CompressedBlockReader(InputStream);
        }

        protected override BlockWriter GetBlockWriter()
        {
            return new UncompressedBlockWriter(OutputStream, BlockSize);
        }

        protected override void MakeActionWithBlock(Block block)
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
