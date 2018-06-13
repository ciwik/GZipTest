using System.IO;

namespace GZipLibrary.Blocks.Writers
{
    public class CompressedBlockWriter : BlockWriter
    {
        public CompressedBlockWriter(Stream stream) : base(stream)
        {
        }

        public override void Write(Block block)
        {
            var data = block.GetBytes();
            Stream.Write(data, 0, data.Length);
        }
    }
}
