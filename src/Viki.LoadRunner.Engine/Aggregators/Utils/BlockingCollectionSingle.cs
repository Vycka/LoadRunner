using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Viki.LoadRunner.Engine.Aggregators.Utils
{
    /// <summary>
    /// Specialized BlockingCollection to increase performence of LoadRunner pipleline.
    ///  * It only works correctly with one producer and one consumer, doing more will break it.
    ///  * its only eventually consistent (things added not always gets read with upcomming read attempts)
    ///  * CompleteAdding doesn't prevent adding more, and as long as you are not fast enough to read it, EnumerableQueue stays active
    ///  </summary>
    public class BlockingCollectionSingle<T> : IEnumerable<T>
    {
        private bool _addingCompleted;

        private Queue<T> _writeOnlyQueue, _readOnlyQueue;

        public BlockingCollectionSingle()
        {
            _writeOnlyQueue = new Queue<T>();
            _readOnlyQueue = new Queue<T>();
        }

        private IEnumerable<T> EnumerableReader
        {
            get
            {
                while (_addingCompleted == false || _readOnlyQueue.Count != 0 || _writeOnlyQueue.Count != 0)
                {
                    while (_readOnlyQueue.Count != 0)
                        yield return _readOnlyQueue.Dequeue();

                    _writeOnlyQueue = Interlocked.Exchange(ref _readOnlyQueue, _writeOnlyQueue);
                    Thread.Sleep(100);
                }
            }
        }

        public void Add(T item)
        {
            _writeOnlyQueue.Enqueue(item);
        }

        public void CompleteAdding()
        {
            _addingCompleted = true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public IEnumerator<T> GetEnumerator()
        {
            return EnumerableReader.GetEnumerator();
        }
    }
}