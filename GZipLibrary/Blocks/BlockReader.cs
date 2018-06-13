using System;
using System.IO;

namespace GZipLibrary.Blocks
{
    public abstract class BlockReader : IDisposable
    {
        protected Stream Stream;
        protected int BlockSize;

        protected BlockReader(Stream stream, int blockSize)
        {
            Stream = stream;
            BlockSize = blockSize;
        }

        public abstract bool Read(out Block block);

        public abstract long GetOriginalFileSize();

        public void Dispose()
        {
            Stream?.Dispose();
        }
    }
}
