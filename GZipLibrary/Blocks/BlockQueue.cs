using System.Collections.Generic;
using System.Threading;

namespace GZipLibrary.Blocks
{
    public class BlockQueue
    {
        private readonly int _queueMaxSize;
        private readonly Queue<Block> _queue = new Queue<Block>();
        private bool _isClosed;

        public BlockQueue(int queueMaxSize)
        {
            _queueMaxSize = queueMaxSize;
        }

        public void Enqueue(Block block)
        {
            lock (_queue)
            {
                while (_queue.Count >= _queueMaxSize)
                {
                    Monitor.Wait(_queue);
                }
                _queue.Enqueue(block);
                Monitor.PulseAll(_queue);
            }
        }

        public Block Dequeue()
        {
            lock (_queue)
            {
                while (_queue.Count == 0)
                {
                    if (_isClosed)
                    {
                        return new Block();
                    }

                    Monitor.Wait(_queue);
                }

                var block = _queue.Dequeue();
                Monitor.PulseAll(_queue);
                return block;
            }
        }

        public bool TryDequeue(out Block block)
        {
            lock (_queue)
            {
                while (_queue.Count == 0)
                {
                    if (_isClosed)
                    {
                        block = new Block();
                        return false;
                    }

                    Monitor.Wait(_queue);
                }

                block = _queue.Dequeue();

                Monitor.PulseAll(_queue);

                return true;
            }
        }

        public void Close()
        {
            lock (_queue)
            {
                _isClosed = true;
                Monitor.PulseAll(_queue);
            }
        }
    }
}
