using System;
using System.IO;

namespace GZipLibrary.Blocks
{
    public abstract class BlockWriter : IDisposable
    {
        protected Stream Stream;
        protected int BlockSize;

        protected BlockWriter(Stream stream, int blockSize)
        {
            Stream = stream;
            BlockSize = blockSize;
        }

        public abstract void Write(Block block);
        //TODO: create method WriteBytes or similar instead of this
        public abstract void SetOriginalFileSize(long size);

        public void Dispose()
        {
            Stream?.Dispose();
        }
    }
}
