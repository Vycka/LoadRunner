using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Collector.Pipeline
{
    public class EnumerablePipe<T> : IEnumerable<T>, IProducer<T>
    {
        private readonly BatchingPipe<T> _pipe = new BatchingPipe<T>();

        public void Produce(T item)
        {
            _pipe.Produce(item);
        }

        public void ProducingCompleted()
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