using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Collector.Pipeline
{
    public class PipeMuxer<T> : IPipeFactory<T>
    {
        private bool _completed = false;

        private readonly ConcurrentQueue<IConsumer<T>> _addQueue = new ConcurrentQueue<IConsumer<T>>();

        private readonly List<IConsumer<T>> _consumers = new List<IConsumer<T>>();
        private readonly List<IConsumer<T>> _lockedConsumers = new List<IConsumer<T>>();

        public BatchingPipe<T> Create()
        {
            BatchingPipe<T> pipe = new BatchingPipe<T>();
            _addQueue.Enqueue(pipe);

            return pipe;
        }

        public IEnumerable<IReadOnlyList<T>> LockBatches()
        {
            while (!_addQueue.IsEmpty && _addQueue.TryDequeue(out var consumer))
                _consumers.Add(consumer);

            for (int i = 0; i < _consumers.Count; i++)
            {
                if (_consumers[i].Available)
                {
                    if (_consumers[i].TryLockBatch(out var batch))
                    {
                        _lockedConsumers.Add(_consumers[i]);
                        yield return batch;
                    }
                }
                else
                {
                    _consumers.RemoveAt(i--);
                }
            }
        }

        public void ReleaseBatches()
        {
            for (int i = 0; i < _lockedConsumers.Count; i++)
            {
                _lockedConsumers[i].ReleaseBatch();
            }

            _lockedConsumers.Clear();
        }

        public void Complete()
        {
            _completed = true;
        }

        public void Reset()
        {
            while (!_addQueue.IsEmpty && _addQueue.TryDequeue(out _))
            {
            }

            _consumers.Clear();
            _lockedConsumers.Clear();

            _completed = false;

        }

        public bool Available => !_completed || _consumers.Count != 0 || _addQueue.IsEmpty == false || _consumers.Any(c => c.Available);
    }
}