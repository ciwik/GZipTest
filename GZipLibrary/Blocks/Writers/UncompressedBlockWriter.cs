using System.IO;

namespace GZipLibrary.Blocks.Writers
{
    public class UncompressedBlockWriter : BlockWriter
    {
        private readonly long _blockSize;

        public UncompressedBlockWriter(Stream stream, long blockSize) : base(stream)
        {
            _blockSize = blockSize;
        }

        public override void Write(Block block)
        {
            try
            { 
                Stream.Seek(_blockSize * block.Id, SeekOrigin.Begin);
                Stream.Write(block.Data, 0, (int)block.Size);
            }
            catch (IOException e)
            {
                throw new FileNotFoundException("Can't write to stream", e);
            }
        }
    }
}
