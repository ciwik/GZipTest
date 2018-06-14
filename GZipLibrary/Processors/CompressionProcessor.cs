using System;
using System.IO;
using System.IO.Compression;
using GZipLibrary.Blocks;
using GZipLibrary.Blocks.Readers;
using GZipLibrary.Blocks.Writers;

namespace GZipLibrary.Processors
{
    public class CompressionProcessor : BaseProcessor
    {
        public CompressionProcessor(Stream inputStream, Stream outputStream, int blockSize, int queueSize) : base(inputStream, outputStream, queueSize)
        {
            BlockSize = blockSize;
        }

        protected override BlockReader GetBlockReader()
        {
            FullUncompressedStreamLength = InputStream.Length;
            return new UncompressedBlockReader(InputStream, BlockSize);
        }

        protected override BlockWriter GetBlockWriter()
        {
            //Write length of the original stream to the first 8 bytes of output stream
            WriteNumberToStream(FullUncompressedStreamLength, OutputStream);
            //Write the block size to the second 8 bytes of output stream
            WriteNumberToStream(BlockSize, OutputStream);

            return new CompressedBlockWriter(OutputStream);
        }

        protected override void MakeActionWithBlock(Block block)
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

        private void WriteNumberToStream(long number, Stream stream)
        {
            var sizeBytes = BitConverter.GetBytes(number);
            stream.Write(sizeBytes, 0, sizeBytes.Length);
        }
    }
}
