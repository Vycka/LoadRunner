using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Collector.Pipeline
{
    public class EnumerablePipeMultiplexer<T> : IProducer<T>
    {
        private readonly EnumerablePipe<T>[] _pipes;

        public EnumerablePipeMultiplexer(int consumerCount)
        {
            if (consumerCount <= 0) throw new ArgumentOutOfRangeException(nameof(consumerCount));

            _pipes = new EnumerablePipe<T>[consumerCount];
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

        public IEnumerable<T> this[int index] => _pipes[index];
    }
}