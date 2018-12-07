using System.Collections.Generic;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Collector.Pipeline
{
    public class BatchingPipe<T> : IProducer<T>, IConsumer<T>
    {
        private bool _completed;

        private List<T> _writeOnlyList, _readOnlyList;

        public BatchingPipe()
        {
            _writeOnlyList = new List<T>();
            _readOnlyList = new List<T>();
        }

        public bool Available => !_completed || _readOnlyList.Count != 0 || _writeOnlyList.Count != 0;

        public bool TryLockBatch(out IReadOnlyList<T> batch)
        {
            bool result = false;

            if (_readOnlyList.Count != 0)
            {
                batch = _readOnlyList;
                result = true;
            }
            else if (_completed && _writeOnlyList.Count != 0)
            {
                batch = _writeOnlyList;
                _writeOnlyList = _readOnlyList;

                _readOnlyList = (List<T>)batch;

                result = true;
            }
            else
            {
                batch = null;
            }

            return result;
        }

        public void ReleaseBatch()
        {
            _readOnlyList.Clear();
        }

        public void Produce(T item)
        {
            _writeOnlyList.Add(item);

            if (_completed == false && _readOnlyList.Count == 0)
            {
                //_writeOnlyList = Interlocked.Exchange(ref _readOnlyList, _writeOnlyList);
                var tmp = _readOnlyList;
                _readOnlyList = _writeOnlyList;
                _writeOnlyList = tmp;
            }
        }

        public void ProducingCompleted()
        {
            _completed = true;
        }

        public void Reset()
        {
            _writeOnlyList.Clear();
            _readOnlyList.Clear();
            _completed = false;
        }
    }
}