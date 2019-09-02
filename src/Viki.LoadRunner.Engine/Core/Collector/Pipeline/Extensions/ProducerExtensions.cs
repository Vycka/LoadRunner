using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Collector.Pipeline.Extensions
{
    public static class ProducerExtensions
    {
        public static Task ProduceCompleteAsync<T>(this IProducer<T> producer, IEnumerable<T> items, bool start = true)
        {
            return ProduceCompleteAsync(producer, items, CancellationToken.None, start);
        }

        public static Task ProduceCompleteAsync<T>(this IProducer<T> producer, IEnumerable<T> items, CancellationToken token, bool start = true)
        {
            Task task = new Task(() => ProduceAndComplete(producer, items, token), token, TaskCreationOptions.LongRunning);

            if (start)
                task.Start();

            return task;
        }

        private static void ProduceAndComplete<T>(IProducer<T> producer, IEnumerable<T> items, CancellationToken token)
        {
            try
            {
                foreach (T item in items)
                {
                    if (token.IsCancellationRequested)
                        break;

                    producer.Produce(item);
                }
            }
            finally
            {
                producer.ProducingCompleted();
            }
        }

        public static void Produce<T>(this IProducer<T> producer, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                producer.Produce(item);
            }
        }
    }
}