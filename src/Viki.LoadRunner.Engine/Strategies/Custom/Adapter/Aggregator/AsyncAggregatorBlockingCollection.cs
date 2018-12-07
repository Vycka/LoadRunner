using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Adapter.Aggregator
{
    /// <summary>
    /// Since TestContextResultReceived calls are synchronous from benchmarking threads, this class unloads processing to its own seperate thread
    /// It's already used in LoadRunnerEngine, so no need to reuse it again.
    /// </summary>
    internal class AsyncAggregatorBlockingCollection : IAggregator
    {
        #region Fields

        private readonly IAggregator[] _aggregators;

        private BlockingCollection<IResult> _queue;
        private Task _aggregateTask;

        #endregion

        #region Constructor

        public AsyncAggregatorBlockingCollection(params IAggregator[] aggregators)
        {
            _aggregators = aggregators;
        }

        #endregion

        #region IAggregator + Process

        public void Begin()
        {
            _queue = new BlockingCollection<IResult>();
            Array.ForEach(_aggregators, aggregator => aggregator.Begin()); 
            _aggregateTask = new Task(() => Aggregate(_queue.GetConsumingEnumerable()));
            _aggregateTask.Start();
        }

        public void Aggregate(IResult result)
        {
            _queue.Add(result);

            if (_aggregateTask.Exception != null)
                throw _aggregateTask.Exception;
        }

        private void Aggregate(IEnumerable<IResult> stream)
        {
            foreach (IResult item in stream)
            {
                Array.ForEach(_aggregators, a => a.Aggregate(item));
            }
        }

        public void End()
        {
            _queue.CompleteAdding();
            _aggregateTask.Wait();

            if (_aggregateTask.Exception != null)
                throw _aggregateTask.Exception;

            Array.ForEach(_aggregators, aggregator => aggregator.End());
        }

        #endregion
    }
}
