using System;
using System.IO;

namespace GZipLibrary.Blocks.Writers
{
    public abstract class BlockWriter : IDisposable
    {
        protected Stream Stream;
        
        protected BlockWriter(Stream stream)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public abstract void Write(Block block);

        public void Dispose()
        {
            Stream?.Dispose();
        }
    }
}
