using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Viki.LoadRunner.Engine.Core.Collector.Pipeline
{
    public class EnumerableQueue<T> : IEnumerable<T>
    {
        private readonly BatchingPipe<T> _pipe = new BatchingPipe<T>();

        public void Add(T item)
        {
            _pipe.Produce(item);
        }

        public void CompleteAdding()
        {
            _pipe.ProducingCompleted();
        }

        private IEnumerable<T> EnumerableReader
        {
            get
            {
                while (_pipe.Available)
                {
                    IReadOnlyList<T> batch;
                    while (_pipe.TryLockBatch(out batch))
                    {
                        for (int i = 0; i < batch.Count; i++)
                        {
                            yield return batch[i];
                        }

                        _pipe.ReleaseBatch();
                    }

                    Thread.Sleep(100);
                }
            }
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