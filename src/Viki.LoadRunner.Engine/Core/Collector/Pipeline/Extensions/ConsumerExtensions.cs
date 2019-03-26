using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Collector.Pipeline.Extensions
{
    public static class ConsumerExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this IConsumer<T> consumer, int sleepMilliseconds = 100)
        {
            while (consumer.Available)
            {
                IReadOnlyList<T> batch;
                while (consumer.TryLockBatch(out batch))
                {
                    for (int i = 0; i < batch.Count; i++)
                    {
                        yield return batch[i];
                    }

                    consumer.ReleaseBatch();
                }

                Thread.Sleep(sleepMilliseconds);
            }
        }

        public static Task ToEnumerableAsync<T>(this IConsumer<T> consumer, Action<IEnumerable<T>> consumeAction, int sleepMilliseconds = 100, bool start = true)
        {
            Task task = new Task(() => consumeAction(consumer.ToEnumerable(sleepMilliseconds)), TaskCreationOptions.LongRunning);

            if (start)
                task.Start();

            return task;
        }
    }
}