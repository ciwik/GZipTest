using System;
using System.IO;
using System.IO.Compression;

namespace GZipTest.CommandLine
{
    public class Parser
    {
        public Arguments Parse(string[] args)
        {
            if (args == null || args.Length < 3)
            {
                //TODO
                throw new Exception();
            }

            CompressionMode compressionMode;
            if (args[0].Equals("compress"))
            {
                compressionMode = CompressionMode.Compress;
            } else if (args[0].Equals("decompress"))
            {
                compressionMode = CompressionMode.Decompress;
            }
            else
            {
                //TODO
                throw new Exception();
            }
            
            var inputFilePath = Path.GetFullPath(args[1]);
            var outputFilePath = Path.GetFullPath(args[2]);           

            return new Arguments(compressionMode, inputFilePath, outputFilePath);            
        }
    }
}
