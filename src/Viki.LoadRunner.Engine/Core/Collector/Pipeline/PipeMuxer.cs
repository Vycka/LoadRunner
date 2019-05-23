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

        private readonly List<T> _buffer = new List<T>();

        public PipeMuxer()
        {
        }
        public PipeMuxer(IEnumerable<IConsumer<T>> consumers)
        {
            Add(consumers);
        }


        public void Complete()
        {
            _completed = true;
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

        public void Reset()
        {
            while (!_addQueue.IsEmpty && _addQueue.TryDequeue(out _))
            {
            }

            ReleaseBatch();

            _consumers.Clear();
            _completed = false;
        }

        #region IConsumer

        public bool Available => !_completed || _consumers.Count != 0 || _buffer.Count != 0 || _addQueue.IsEmpty == false;

        public bool TryLockBatch(out IReadOnlyList<T> batch)
        {
            _buffer.AddRange(TryRead().SelectMany(r => r));

            bool result = _buffer.Count != 0;

            batch = result ? _buffer : null;

            return result;
        }

        public void ReleaseBatch()
        {
            _buffer.Clear();
            Release();
        }


        #endregion

        private IEnumerable<IReadOnlyList<T>> TryRead()
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

        private void Release()
        {
            for (int i = 0; i < _lockedConsumers.Count; i++)
            {
                _lockedConsumers[i].ReleaseBatch();
            }

            _lockedConsumers.Clear();
        }
    }
}