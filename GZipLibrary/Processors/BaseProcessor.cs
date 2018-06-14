using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using GZipLibrary.Blocks;
using GZipLibrary.Blocks.Readers;
using GZipLibrary.Blocks.Writers;

namespace GZipLibrary.Processors
{
    public abstract class BaseProcessor : IProcessor
    {
        private readonly BlockQueue _readBlockQueue, _writeBlockQueue;
        private Dictionary<Thread, ManualResetEvent> _actionThreads;
        private bool _isCancelled;

        protected Stream InputStream, OutputStream;
        protected long BlockSize, FullUncompressedStreamLength;

        protected BaseProcessor(Stream inputStream, Stream outputStream, int queueSize)
        {
            InputStream = inputStream;
            OutputStream = outputStream;

            _readBlockQueue = new BlockQueue(queueSize);
            _writeBlockQueue = new BlockQueue(queueSize);
        }

        public void Run()
        {
            //One of the physical threads is given for reading, another one - for writing, all other threads - for compression.
            //If the number of the physical threads in the system are less than two, then use one thread for compression
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
                _actionThreads.Add(thread, new ManualResetEvent(false)); ;
                thread.Start();
            }

            readingThread.Join();
            WaitHandle[] waitHandles = _actionThreads.Values.ToArray();
            WaitHandle.WaitAll(waitHandles);
            _writeBlockQueue.Close();
            writingThread.Join();
        }

        public void Cancel()
        {
            _isCancelled = true;
        }

        protected abstract BlockReader GetBlockReader();
        protected abstract BlockWriter GetBlockWriter();
        protected abstract void MakeActionWithBlock(Block block);

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

        private Thread CreateThread(ThreadStart action)
        {
            return new Thread(action)
            {
                //Use a priority below normal so as not to interfere with Garbage Collector
                Priority = ThreadPriority.BelowNormal,
                IsBackground = true
            };
        }

        private void MakeActionWithNextBlock()
        {
            while (!_isCancelled && _readBlockQueue.TryDequeue(out Block block))
            {
                MakeActionWithBlock(block);
                _writeBlockQueue.Enqueue(block);
            }

            _actionThreads[Thread.CurrentThread].Set();
        }   
    }
}
