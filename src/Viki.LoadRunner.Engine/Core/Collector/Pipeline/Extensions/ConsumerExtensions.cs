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


        // This one day might evolve into fun multithreaded processing pipeline.
        //public static IEnumerable<TOut> SelectManyInParallel<TIn, TOut>(this IEnumerable<IConsumer<TIn>> consumers, Func<TIn, TOut> selector)
        //{
        //    PipeMuxer<TOut> muxer = new PipeMuxer<TOut>();

        //    foreach (IConsumer<TIn> consumer in consumers)
        //    {
        //        IProducer<TOut> pipe = muxer.Create();
        //        pipe.ProduceCompleteAsync(consumer.ToEnumerable().Select(selector));
        //    }

        //    return muxer.ToEnumerable().SelectMany(batch => batch);
        //}

        //public static IEnumerable<TOut> SelectManyInParallel<TIn, TOut>(this IEnumerable<IConsumer<TIn>> consumers, Func<IEnumerable<TIn>, IEnumerable<TOut>> selector)
        //{
        //    PipeMuxer<TOut> muxer = new PipeMuxer<TOut>();

        //    foreach (IConsumer<TIn> consumer in consumers)
        //    {
        //        IProducer<TOut> pipe = muxer.Create();
        //        pipe.ProduceCompleteAsync(selector(consumer.ToEnumerable()));
        //    }

        //    return muxer.ToEnumerable().SelectMany(batch => batch);
        //}
    }
}