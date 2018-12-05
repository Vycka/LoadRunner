using System.Collections.Generic;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Collector.Pipeline
{
    public class BatchingQueue<T> : IProducer<T>, IConsumer<T>
    {
        public bool Completed { get; private set; }

        private List<T> _writeOnlyList, _readOnlyList;

        public BatchingQueue()
        {
            _writeOnlyList = new List<T>();
            _readOnlyList = new List<T>();
        }

        public IReadOnlyList<T> TryLockBatch()
        {
            if (_readOnlyList.Count != 0)
                return _readOnlyList;

            if (Completed)
                return _writeOnlyList;

            return null;
        }

        public void ReleaseBatch()
        {
            _readOnlyList.Clear();
        }

        public void Produce(T item)
        {
            _writeOnlyList.Add(item);

            if (Completed == false && _readOnlyList.Count == 0)
                _writeOnlyList = Interlocked.Exchange(ref _readOnlyList, _writeOnlyList);
        }

        public void FinishProducing()
        {
            Completed = true;
        }

        public void Reset()
        {
            _writeOnlyList.Clear();
            _readOnlyList.Clear();
            Completed = false;
        }
    }
}