using System;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Executor.Collector;
using Viki.LoadRunner.Engine.Executor.Collector.Interfaces;
using Viki.LoadRunner.Engine.Executor.Counter.Interfaces;
using Viki.LoadRunner.Engine.Executor.Factory.Interfaces;
using Viki.LoadRunner.Engine.Executor.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Factory
{
    public class DataCollectorFactory : IDataCollectorFactory
    {
        private readonly IResultsAggregator _aggregator;
        private readonly IThreadPoolCounter _counter;

        public DataCollectorFactory(IResultsAggregator aggregator, IThreadPoolCounter counter)
        {
            if (aggregator == null)
                throw new ArgumentNullException(nameof(aggregator));
            if (counter == null)
                throw new ArgumentNullException(nameof(counter));

            _aggregator = aggregator;
            _counter = counter;
        }

        public IDataCollector Create(IIterationResult iterationContext)
        {
            IDataCollector collector = new DataCollector(iterationContext, _aggregator, _counter);

            return collector;
        }
    }
}