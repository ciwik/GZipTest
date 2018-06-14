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

            try
            {
                Stream.Write(data, 0, data.Length);
            }
            catch (IOException e)
            {
                throw new FileNotFoundException("Can't write to stream", e);
            }
        }
    }
}
