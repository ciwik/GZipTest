using System;
using System.IO;

namespace GZipLibrary.Blocks
{
    public abstract class BlockWriter : IDisposable
    {
        protected Stream Stream;
        
        protected BlockWriter(Stream stream)
        {
            Stream = stream;
        }

        public abstract void Write(Block block);

        public void Dispose()
        {
            Stream?.Dispose();
        }
    }
}
