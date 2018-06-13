using System;
using System.IO;

namespace GZipLibrary.Blocks.Readers
{
    public class UncompressedBlockReader : BlockReader
    {
        private int _currentBlockId;

        public UncompressedBlockReader(Stream stream, int blockSize) : base(stream, blockSize)
        {
        }

        public override bool Read(out Block block)
        {
            var blockSize = Math.Min(BlockSize, Stream.Length - Stream.Position);
            var buffer = new byte[blockSize];

            if (Stream.Read(buffer, 0, buffer.Length) > 0)
            {
                block = new Block(_currentBlockId++, buffer);
                return true;
            }

            block = null;
            return false;
        }

        public override long GetOriginalFileSize()
        {
            return Stream.Length;
        }
    }
}
