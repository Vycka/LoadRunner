using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Engine.Strategies.Extensions;
using Viki.LoadRunner.Playground.Tools;

namespace Viki.LoadRunner.Playground
{
    public class AssertPipeline
    {
        public static void Run()
        {
            HistogramAggregator aggregator = new HistogramAggregator()
                .Add(new CountMetric());

            StreamAggregator streamAggregator = new StreamAggregator(CountStream);

            CountingScenarioFactory factory = new CountingScenarioFactory();

            StrategyBuilder strategy = new StrategyBuilder()
                .SetScenario(factory)
                .SetThreading(new FixedThreadCount(4))
                .SetLimit(new TimeLimit(TimeSpan.FromSeconds(15)))
                .SetAggregator(aggregator, streamAggregator);

            strategy.Build().Run();

            Console.WriteLine(JsonConvert.SerializeObject(aggregator.BuildResultsObjects(), Formatting.Indented));
            factory.PrintSum();
            int expected = factory.GetSum();
            int actual = (int)aggregator.BuildResults().Data[0][1];

            Console.WriteLine();
            Console.WriteLine($@"{_streamCount}/{expected}/{actual} ? {expected==actual}");
        }


        private static int _streamCount = 0;
        private static void CountStream(IEnumerable<IResult> input)
        {
            _streamCount = input.Count();
        }
    }
}