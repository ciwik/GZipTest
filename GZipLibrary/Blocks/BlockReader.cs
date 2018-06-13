using System;
using System.IO;

namespace GZipLibrary.Blocks
{
    public abstract class BlockReader : IDisposable
    {
        protected Stream Stream;

        protected BlockReader(Stream stream)
        {
            Stream = stream;
        }

        public abstract bool Read(out Block block);

        public void Dispose()
        {
            Stream?.Dispose();
        }
    }
}
