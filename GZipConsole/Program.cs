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

        private static IProcessor _processor;

        public static void Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;

            var arguments = new CommandLine.Parser().Parse(args);
            _processor = GetProcessor(arguments);
            Stopwatch stopwatch = Stopwatch.StartNew();
            _processor.Run();
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);

            Console.ReadKey();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (e.SpecialKey == ConsoleSpecialKey.ControlC)
            {
                e.Cancel = true;
                _processor.Cancel();
            }
        }

        //TODO: move method to another class
        private static IProcessor GetProcessor(CommandLine.Arguments arguments)
        {
            switch (arguments.CompressionMode)
            {
                case CompressionMode.Compress:
                    return new CompressionProcessor(arguments.InputFilePath, arguments.OutputFilePath, BlockSize, QueueSize);
                case CompressionMode.Decompress:
                    return new DecompressionProcessor(arguments.InputFilePath, arguments.OutputFilePath, QueueSize);
            }
            //TODO
            throw new Exception();
        }
    }
}
