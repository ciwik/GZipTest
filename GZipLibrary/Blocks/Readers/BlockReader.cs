using System;
using System.IO;

namespace GZipLibrary.Blocks.Readers
{
    public abstract class BlockReader : IDisposable
    {
        protected Stream Stream;

        protected BlockReader(Stream stream)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public abstract bool Read(out Block block);

        public void Dispose()
        {
            Stream?.Dispose();
        }
    }
}
