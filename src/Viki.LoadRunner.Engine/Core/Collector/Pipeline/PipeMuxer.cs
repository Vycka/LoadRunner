using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Collector.Pipeline
{
    public class PipeMuxer<T> : IConsumer<IReadOnlyList<T>>
    {
        private bool _completed = false;

        private readonly ConcurrentQueue<IConsumer<T>> _addQueue = new ConcurrentQueue<IConsumer<T>>();

        private readonly List<IConsumer<T>> _consumers = new List<IConsumer<T>>();
        private readonly List<IConsumer<T>> _lockedConsumers = new List<IConsumer<T>>();
        private readonly List<IReadOnlyList<T>> _lockedBatches = new List<IReadOnlyList<T>>();

        public bool Available => !_completed || _consumers.Count != 0 || _addQueue.IsEmpty == false || _consumers.Any(c => c.Available);

        public bool TryLockBatch(out IReadOnlyList<IReadOnlyList<T>> batch)
        {
            

            bool result = LockPipes();
            if (result)
            {
                batch = _lockedBatches;
            }
            else
            {
                batch = null;
            }

            return result;
        }

        public void ReleaseBatch()
        {
            ReleaseConsumers();
        }

        public void Add(IConsumer<T> pipe)
        {
            _addQueue.Enqueue(pipe);
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
            _lockedBatches.Clear();

            _completed = false;

        }

        private bool LockPipes()
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
                        _lockedBatches.Add(batch);
                    }
                }
                else
                {
                    _consumers.RemoveAt(i--);
                }
            }

            return _lockedConsumers.Count != 0;
        }

        private void ReleaseConsumers()
        {
            for (int i = 0; i < _lockedConsumers.Count; i++)
            {
                _lockedConsumers[i].ReleaseBatch();
            }

            _lockedConsumers.Clear();
            _lockedBatches.Clear();
        }
    }
}