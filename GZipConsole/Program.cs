using System;
using System.Diagnostics;
using System.IO;
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

            try
            {
                var arguments = new CommandLine.Parser().Parse(args);
                _processor = GetProcessor(arguments);

                var stopwatch = Stopwatch.StartNew();
                _processor.Run();
                stopwatch.Stop();

                Console.WriteLine($"Work done in {stopwatch.ElapsedMilliseconds} ms");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

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

        private static IProcessor GetProcessor(CommandLine.Arguments arguments)
        {
            var inputStream = GetInputStream(arguments.InputFilePath);
            var outputStream = GetOutputStream(arguments.OutputFilePath);

            switch (arguments.CompressionMode)
            {
                case CompressionMode.Compress:
                    return new CompressionProcessor(inputStream, outputStream, BlockSize, QueueSize);
                case CompressionMode.Decompress:
                    return new DecompressionProcessor(inputStream, outputStream, QueueSize);
                default:
                    throw new ArgumentException($"Parameter \"{nameof(arguments.CompressionMode)}\" is wrong", nameof(arguments.CompressionMode));
            }                      
        }

        private static Stream GetInputStream(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File \"{filePath}\" not found", filePath);
            }

            var inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return inputStream;
        }

        private static Stream GetOutputStream(string filePath)
        {
            if (File.Exists(filePath))
            {
                Console.WriteLine($"File \"{filePath}\" already exists. Do you want to overwrite it? y/n");
                var fileShouldBeOverwritten = GetUserResponse();

                if (fileShouldBeOverwritten)
                {
                    return new FileStream(filePath, FileMode.Truncate, FileAccess.Write);
                }

                throw new FileLoadException("File can't be loaded");
            }

            return new FileStream(filePath, FileMode.Create, FileAccess.Write);
        }

        private static bool GetUserResponse()
        {
            string response = string.Empty;

            while (true)
            {
                response = Console.ReadLine();
                if (response == null)
                {
                    continue;
                }

                response = response.ToLower();

                if (response.Equals("y"))
                {
                    return true;
                }

                if (response.Equals("n"))
                {
                    return false;
                }
            }
        }       
    }
}
