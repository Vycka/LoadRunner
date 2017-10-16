using System;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Executor.Collector.Interfaces;
using Viki.LoadRunner.Engine.Executor.Counter.Interfaces;
using Viki.LoadRunner.Engine.Executor.Pool.Interfaces;
using Viki.LoadRunner.Engine.Executor.Scenario.Interfaces;

#pragma warning disable 1591

namespace Viki.LoadRunner.Engine.Executor.Collector
{
    public class DataCollector : IDataCollector
    {
        private readonly IIterationResult _context;
        private readonly IResultsAggregator _aggregator;
        private readonly IThreadPoolStats _poolStats;

        public DataCollector(IIterationResult context, IResultsAggregator aggregator, IThreadPoolCounter poolStats)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (aggregator == null)
                throw new ArgumentNullException(nameof(aggregator));
            if (poolStats == null)
                throw new ArgumentNullException(nameof(poolStats));

            _context = context;
            _aggregator = aggregator;
            _poolStats = poolStats;
        }

        public void Collect()
        {
            _aggregator.TestContextResultReceived(new IterationResult(_context, _poolStats));
        }
    }
}