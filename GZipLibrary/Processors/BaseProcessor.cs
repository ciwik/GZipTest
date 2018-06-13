using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GZipLibrary.Blocks;

namespace GZipLibrary.Processors
{
    public abstract class BaseProcessor : IProcessor
    {
        private readonly BlockQueue _readBlockQueue, _writeBlockQueue;
        private Dictionary<Thread, ManualResetEvent> _actionThreads;
        private bool _isCancelled;

        protected string InputFilePath, OutputFilePath;
        protected long BlockSize, FullFileSize;
        
        protected BaseProcessor(string inputFilePath, string outputFilePath, int queueSize)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;

            _readBlockQueue = new BlockQueue(queueSize);
            _writeBlockQueue = new BlockQueue(queueSize);
        }

        private void Read(BlockReader reader)
        {
            while (!_isCancelled && reader.Read(out Block block))
            {
                _readBlockQueue.Enqueue(block);
            }

            _readBlockQueue.Close();
            reader.Dispose();
        }

        private void Write(BlockWriter writer)
        {
            while (!_isCancelled && _writeBlockQueue.TryDequeue(out Block block))
            {
                writer.Write(block);
            }

            writer.Dispose();
        }

        private Thread CreateThread (ThreadStart action)
        {
            return new Thread(action)
            {
                Priority = ThreadPriority.BelowNormal,
                IsBackground = true
            };
        }

        private void MakeActionWithNextBlock()
        {
            while (!_isCancelled && _readBlockQueue.TryDequeue(out Block block))
            {
                DoActionWithBlock(block);
                _writeBlockQueue.Enqueue(block);
            }

            _actionThreads[Thread.CurrentThread].Set();
        }

        protected abstract BlockReader GetBlockReader();
        protected abstract BlockWriter GetBlockWriter();
        protected abstract void DoActionWithBlock(Block block);

        public void Run()
        {
            int compressionThreadsNumber = (Environment.ProcessorCount > 2) ? Environment.ProcessorCount - 2 : 1;
            _actionThreads = new Dictionary<Thread, ManualResetEvent>();

            var blockReader = GetBlockReader();
            var blockWriter = GetBlockWriter();

            var readingThread = CreateThread(() => Read(blockReader));
            var writingThread = CreateThread(() => Write(blockWriter));

            readingThread.Start();
            writingThread.Start();

            for (int i = 0; i < compressionThreadsNumber; i++)
            {
                var thread = CreateThread(MakeActionWithNextBlock);
                _actionThreads.Add(thread, new ManualResetEvent(false));;             
                thread.Start();
            }
            
            //TODO: Stopwatch stopwatch = Stopwatch.StartNew();
            readingThread.Join();
            WaitHandle.WaitAll(_actionThreads.Values.ToArray());
            _writeBlockQueue.Close();            
            writingThread.Join();            
        }

        public void Cancel()
        {
            _isCancelled = true;
        }
    }
}
