using System;
using System.IO;

namespace GZipLibrary.Blocks.Writers
{
    public class CompressedBlockWriter : BlockWriter
    {
        public CompressedBlockWriter(Stream stream, int blockSize) : base(stream, blockSize)
        {
        }

        public override void Write(Block block)
        {
            var data = block.GetBytes();
            Stream.Write(data, 0, data.Length);
        }

        public override void SetOriginalFileSize(long size)
        {
            Stream.Seek(0, SeekOrigin.Begin);
            var sizeBytes = BitConverter.GetBytes(size);
            Stream.Write(sizeBytes, 0, sizeBytes.Length);
        }
    }
}
