using System;
using System.IO;

namespace GZipLibrary.Blocks.Readers
{
    public class CompressedBlockReader : BlockReader
    {
        public CompressedBlockReader(Stream stream) : base(stream)
        {
        }        

        public override bool Read(out Block block)
        {
            var buffer = new byte[2 * sizeof(long)];
            Stream.Read(buffer, 0, buffer.Length);
            var id = BitConverter.ToInt64(buffer, 0);
            var size = BitConverter.ToInt64(buffer, sizeof(long));

            buffer = new byte[size];
            if (Stream.Read(buffer, 0, buffer.Length) > 0)
            {
                block = new Block(id, buffer);
                return true;
            }

            block = new Block();
            return false;
        }
    }
}
