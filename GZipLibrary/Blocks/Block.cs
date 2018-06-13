using System;

namespace GZipLibrary.Blocks
{
    public class Block
    {
        public long Id { get; }
        public byte[] Data { get; set; }
        public long Size => Data.LongLength;

        public Block()
        {
            Id = -1;
            Data = new byte[0];
        }

        public Block(long id, byte[] data)
        {
            Id = id;
            Data = data;
        }

        public byte[] GetBytes()
        {
            var result = new byte[Data.Length + 2 * sizeof(long)];

            var idBytes = BitConverter.GetBytes(Id);
            var sizeBytes = BitConverter.GetBytes(Size);

            Buffer.BlockCopy(idBytes, 0, result, 0, idBytes.Length);
            Buffer.BlockCopy(sizeBytes, 0, result, idBytes.Length, sizeBytes.Length);
            Buffer.BlockCopy(Data, 0, result, idBytes.Length + sizeBytes.Length, Data.Length);

            return result;
        }
    }
}
