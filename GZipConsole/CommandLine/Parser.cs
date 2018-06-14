using System;
using System.IO;
using System.IO.Compression;

namespace GZipTest.CommandLine
{
    public class Parser
    {
        private const string CompressionCommand = "compress";
        private const string DecompressionCommand = "decompress";

        public Arguments Parse(string[] args)
        {
            if (args == null || args.Length < 3)
            {
                throw new ArgumentException("At least 3 arguments should be passed");
            }

            CompressionMode compressionMode;
            if (args[0].Equals(CompressionCommand))
            {
                compressionMode = CompressionMode.Compress;
            } else if (args[0].Equals(DecompressionCommand))
            {
                compressionMode = CompressionMode.Decompress;
            }
            else
            {
                throw new ArgumentException($"\"{args[0]}\" is invalid command. Use \"{CompressionCommand}\" or \"{DecompressionCommand}\" commands");
            }

            var inputFilePath = Path.GetFullPath(args[1]);
            var outputFilePath = Path.GetFullPath(args[2]);           

            return new Arguments(compressionMode, inputFilePath, outputFilePath);            
        }
    }
}
