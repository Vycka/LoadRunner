using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Collector.Pipeline
{
    public class PipeMuxer<T> : IConsumer<T>
    {
        private bool _completed = false;

        private readonly ConcurrentQueue<IConsumer<T>> _addQueue = new ConcurrentQueue<IConsumer<T>>();

        private readonly List<IConsumer<T>> _consumers = new List<IConsumer<T>>();
        private readonly List<IConsumer<T>> _lockedConsumers = new List<IConsumer<T>>();
        private readonly List<IReadOnlyList<T>> _lockedBatches = new List<IReadOnlyList<T>>();

        private readonly List<T> _buffer = new List<T>();

        public PipeMuxer()
        {
        }
        public PipeMuxer(IEnumerable<IConsumer<T>> consumers)
        {
            Add(consumers);
        }

        public bool Available => !_completed || _consumers.Count != 0 || _buffer.Count != 0 || _addQueue.IsEmpty == false;

        public bool TryLockBatch(out IReadOnlyList<T> batch)
        {
            bool result = TryLockPipes();
            if (result)
            {
                _buffer.AddRange(_lockedBatches.SelectMany(b => b));
                batch = _buffer;
            }
            else
            {
                batch = null;
            }

            return result;
        }

        public void ReleaseBatch()
        {
            _buffer.Clear();
            ReleasePipes();
        }

        public IProducer<T> Create()
        {
            BatchingPipe<T> pipe = new BatchingPipe<T>();

            Add(pipe);

            return pipe;
        }

        public void Add(IEnumerable<IConsumer<T>> consumers)
        {
            foreach (IConsumer<T> consumer in consumers)
            {
                Add(consumer);
            }
        }

        public void Add(IConsumer<T> consumers)
        {
            _addQueue.Enqueue(consumers);
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

            ReleasePipes();

            _consumers.Clear();
            _completed = false;
        }

        private bool TryLockPipes()
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

        private void ReleasePipes()
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