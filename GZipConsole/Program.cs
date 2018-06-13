using System;
using System.Diagnostics;
using System.IO.Compression;
using GZipLibrary.Processors;

namespace GZipTest
{
    public class Program
    {
        private const int QueueSize = 64;
        private const int BlockSize = 64 * 1024 * 1024;

        public static void Main(string[] args)
        {
            var arguments = new CommandLine.Parser().Parse(args);
            var processor = GetProcessor(arguments);
            Stopwatch stopwatch = Stopwatch.StartNew();
            processor.Run();
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);

            Console.ReadKey();
        }

        //TODO: move method to another class
        private static IProcessor GetProcessor(CommandLine.Arguments arguments)
        {
            switch (arguments.CompressionMode)
            {
                case CompressionMode.Compress:
                    return new CompressionProcessor(arguments.InputFilePath, arguments.OutputFilePath, BlockSize, QueueSize);
                case CompressionMode.Decompress:
                    return new DecompressionProcessor(arguments.InputFilePath, arguments.OutputFilePath, BlockSize, QueueSize);
            }
            //TODO
            throw new Exception();
        }
    }
}
