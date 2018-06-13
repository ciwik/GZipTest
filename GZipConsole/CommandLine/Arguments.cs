using System.IO.Compression;

namespace GZipTest.CommandLine
{
    public class Arguments
    {
        public CompressionMode CompressionMode { get; }
        public string InputFilePath { get; }
        public string OutputFilePath { get; }

        public Arguments(CompressionMode compressionMode, string inputFilePath, string outputFilePath)
        {
            CompressionMode = compressionMode;
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
        }
    }
}
