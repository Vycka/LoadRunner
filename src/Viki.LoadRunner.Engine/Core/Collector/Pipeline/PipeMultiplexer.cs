using System;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Collector.Pipeline
{
    public class PipeMultiplexer<T> : IProducer<T>
    {
        private readonly Func<T, int> _partitionSelector;
        private readonly BatchingPipe<T>[] _pipes;

        public PipeMultiplexer(int consumerCount, Func<T, int> partitionSelector)
        {
            if (consumerCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(consumerCount));
            _partitionSelector = partitionSelector ?? throw new ArgumentNullException(nameof(partitionSelector));

            _pipes = new BatchingPipe<T>[consumerCount];
            for (int i = 0; i < consumerCount; i++)
            {
                _pipes[i] = new BatchingPipe<T>();
            }
        }

        public void Produce(T item)
        {
            int partition = _partitionSelector(item) % _pipes.Length;

            _pipes[partition].Produce(item);
        }

        public void ProducingCompleted()
        {
            for (int i = 0; i < _pipes.Length; i++)
            {
                _pipes[i].ProducingCompleted();
            }
        }

        public IConsumer<T> this[int index] => _pipes[index];
    }
}