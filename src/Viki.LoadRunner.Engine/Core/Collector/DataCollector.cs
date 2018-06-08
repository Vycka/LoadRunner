using System;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Counter.Interfaces;
using Viki.LoadRunner.Engine.Core.Pool.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Collector
{
    public class DataCollector : IDataCollector
    {
        private readonly IIterationResult _context;
        private readonly IAggregator _aggregator;
        private readonly IThreadPoolStats _poolStats;

        public DataCollector(IIterationResult context, IAggregator aggregator, IThreadPoolCounter poolStats)
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
            _aggregator.Aggregate(new IterationResult(_context, _poolStats));
        }
    }
}