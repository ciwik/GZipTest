using System;
using System.IO;

namespace GZipLibrary.Blocks.Readers
{
    public class UncompressedBlockReader : BlockReader
    {
        private int _currentBlockId;
        private readonly long _blockSize;

        public UncompressedBlockReader(Stream stream, long blockSize) : base(stream)
        {
            _blockSize = blockSize;
        }

        public override bool Read(out Block block)
        {
            var blockSize = Math.Min(_blockSize, Stream.Length - Stream.Position);
            var buffer = new byte[blockSize];

            try
            { 
                if (Stream.Read(buffer, 0, buffer.Length) > 0)
                {
                    block = new Block(_currentBlockId++, buffer);
                    return true;
                }
            }
            catch (IOException e)
            {
                throw new FileNotFoundException("Can't read from stream", e);
            }

            block = null;
            return false;
        }
    }
}
