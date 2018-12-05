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
                result = true;
            }
            else
            {
                batch = null;
            }

            return result;

            //bool completedAtStart = _completed;

            //bool result = false;

            //if (_readOnlyList.Count != 0)
            //{
            //    batch = _readOnlyList;
            //    result = true;
            //}
            //else if (completedAtStart)
            //{
            //    if (_readOnlyList.Count != 0)
            //    {
            //        batch = _readOnlyList;
            //    }
            //    else if (_writeOnlyList.Count != 0)
            //    {
            //        batch = _writeOnlyList;
            //    }
            //    else
            //    {
            //        batch = null;
            //    }

            //    result = _readOnlyList.Count != 0 || _writeOnlyList.Count != 0;
            //}
            //// ReSharper disable once ConditionIsAlwaysTrueOrFalse
            //// Other thread can change Completed state
            //// and if thats the case, we need to do a single recursion
            //else if (completedAtStart != _completed)
            //{
            //    result = TryLockBatch(out batch);
            //}
            //else
            //{
            //    batch = null;
            //}

            //return result;
        }

        public void ReleaseBatch()
        {
            _readOnlyList.Clear();
        }

        public void Produce(T item)
        {
            _writeOnlyList.Add(item);

            if (_completed == false && _readOnlyList.Count == 0)
                _writeOnlyList = Interlocked.Exchange(ref _readOnlyList, _writeOnlyList);
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