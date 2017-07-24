using System;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Result;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;

#pragma warning disable 1591

namespace Viki.LoadRunner.Engine.Executor.Threads.Stats
{
    public class DataCollector : IDataCollector
    {
        private readonly IResultsAggregator _aggregator;
        private readonly IThreadPoolStats _stats;

        public DataCollector(IResultsAggregator aggregator, IThreadPoolCounter stats)
        {
            if (aggregator == null)
                throw new ArgumentNullException(nameof(aggregator));
            if (stats == null)
                throw new ArgumentNullException(nameof(stats));

            _aggregator = aggregator;
            _stats = stats;
        }

        public void Collect(ITestContextResult result)
        {
            _aggregator.TestContextResultReceived(new IterationResult(result, _stats));
        }
    }
}