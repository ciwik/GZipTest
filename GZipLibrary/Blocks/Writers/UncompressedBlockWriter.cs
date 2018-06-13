using System.IO;

namespace GZipLibrary.Blocks.Writers
{
    public class UncompressedBlockWriter : BlockWriter
    {
        public UncompressedBlockWriter(Stream stream, int blockSize) : base(stream, blockSize)
        {
        }

        public override void Write(Block block)
        {
            Stream.Seek(BlockSize * block.Id, SeekOrigin.Begin);
            Stream.Write(block.Data, 0, (int)block.Size);
        }
        
        public override void SetOriginalFileSize(long size)
        {
            //TODO
            throw new System.NotImplementedException();
        }
    }
}
