using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Engine.Strategies.Extensions;

namespace Viki.LoadRunner.Playground
{
    public class BlankStressScenarioMemoryStream 
    {
        public static void Run()
        {
            IEnumerable<IResult> rawResults = null;
            StreamAggregator streamAggregator = new StreamAggregator(results => rawResults = results);

            HistogramAggregator aggregator = CreateAggregator();

            HistogramAggregator aggregagorOriginal = CreateAggregator();

            StrategyBuilder strategy = new StrategyBuilder()
                .SetScenario<BlankScenario>()
                .SetThreading(new FixedThreadCount(3))
                .SetLimit(new TimeLimit(TimeSpan.FromSeconds(5)))
                .SetAggregator(streamAggregator, aggregagorOriginal);

            strategy.Build().Run();

            StreamAggregator.Replay(rawResults, aggregator);

            Console.WriteLine(@"-------- FROM LIVE AGGREGATION --------");
            Console.WriteLine(JsonConvert.SerializeObject(aggregagorOriginal.BuildResultsObjects(), Formatting.Indented));
            Console.WriteLine(@"-------- FROM STREAM --------");
            Console.WriteLine(JsonConvert.SerializeObject(aggregator.BuildResultsObjects(), Formatting.Indented));
        }

        private static HistogramAggregator CreateAggregator()
        {
            return new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(2)))
                .Add(new CountMetric())
                .Add(new TransactionsPerSecMetric());
        }
    }
}
