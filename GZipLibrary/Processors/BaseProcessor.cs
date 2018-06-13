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
        private Dictionary<Thread, ManualResetEvent> _compressionThreads;
        private bool _isCancelled;

        protected string InputFilePath, OutputFilePath;
        protected int BlockSize;
        
        protected BaseProcessor(string inputFilePath, string outputFilePath, int blockSize, int queueSize)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
            BlockSize = blockSize;

            _readBlockQueue = new BlockQueue(queueSize);
            _writeBlockQueue = new BlockQueue(queueSize);
        }

        protected long LastBlockSize;
        protected long BlocksCount;
        private long _fullFileSize;
        private void Read()
        {
            using (var reader = GetBlockReader())
            {
                _fullFileSize = reader.GetOriginalFileSize();
                LastBlockSize = _fullFileSize % BlockSize;
                BlocksCount = _fullFileSize / BlockSize + (LastBlockSize == 0 ? 0 : 1);

                while (!_isCancelled && reader.Read(out Block block))
                {
                    _readBlockQueue.Enqueue(block);
                }

                _readBlockQueue.Close();
            }
        }

        private void Write()
        {
            using (var writer = GetBlockWriter())
            {
                if (this is CompressionProcessor)
                {
                    writer.SetOriginalFileSize(_fullFileSize);
                }
                while (!_isCancelled && _writeBlockQueue.TryDequeue(out Block block))
                {
                    writer.Write(block);
                }
            }
        }

        private Thread CreateThread (ThreadStart action)
        {
            return new Thread(action)
            {
                Priority = ThreadPriority.AboveNormal,
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

            _compressionThreads[Thread.CurrentThread].Set();
        }

        protected abstract BlockReader GetBlockReader();
        protected abstract BlockWriter GetBlockWriter();
        protected abstract void DoActionWithBlock(Block block);

        public void Run()
        {
            int compressionThreadsNumber = (Environment.ProcessorCount > 2) ? Environment.ProcessorCount - 2 : 1;
            _compressionThreads = new Dictionary<Thread, ManualResetEvent>();

            var readingThread = CreateThread(Read);
            var writingThread = CreateThread(Write);

            readingThread.Start();
            writingThread.Start();

            for (int i = 0; i < compressionThreadsNumber; i++)
            {
                var thread = CreateThread(MakeActionWithNextBlock);
                _compressionThreads.Add(thread, new ManualResetEvent(false));;             
                thread.Start();
            }
            
            //TODO: Stopwatch stopwatch = Stopwatch.StartNew();
            readingThread.Join();
            WaitHandle.WaitAll(_compressionThreads.Values.ToArray());
            _writeBlockQueue.Close();            
            writingThread.Join();            
        }

        public void Cancel()
        {
            _isCancelled = true;
        }
    }
}
