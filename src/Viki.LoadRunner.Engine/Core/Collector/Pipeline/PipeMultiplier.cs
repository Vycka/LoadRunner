using System;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Collector.Pipeline
{
    public class PipeMultiplier<T> : IProducer<T>
    {
        private readonly BatchingPipe<T>[] _pipes;

        public PipeMultiplier(int consumerCount)
        {
            if (consumerCount <= 0) throw new ArgumentOutOfRangeException(nameof(consumerCount));

            _pipes = new BatchingPipe<T>[consumerCount];
            for (int i = 0; i < consumerCount; i++)
            {
                _pipes[i] = new BatchingPipe<T>();
            }
        }

        public void Produce(T item)
        {
            for (int i = 0; i < _pipes.Length; i++)
            {
                _pipes[i].Produce(item);
            }
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