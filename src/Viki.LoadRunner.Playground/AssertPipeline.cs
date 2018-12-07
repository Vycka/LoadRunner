using System;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Engine.Strategies.Extensions;
using Viki.LoadRunner.Playground.Tools;

namespace Viki.LoadRunner.Playground
{
    public class AssertPipeline
    {
        //private static IResult[] _rawResults;

        public static void Run()
        {
            HistogramAggregator aggregator = new HistogramAggregator()
                .Add(new CountMetric())
                .Add(new FuncMetric<TimeSpan>("max", TimeSpan.Zero, (s, result) => s < result.IterationStarted ? result.IterationStarted : s));

            CountingScenarioFactory factory = new CountingScenarioFactory();

            StrategyBuilder strategy = new StrategyBuilder()
                .SetScenario(factory)
                .SetThreading(new FixedThreadCount(4))
                .SetLimit(new TimeLimit(TimeSpan.FromSeconds(10)))
                //.SetAggregator(new StreamAggregator(results => _rawResults = results.ToArray()));
                .SetAggregator(aggregator);

           
            strategy.Build().Run();

            //StreamAggregator.Replay(_rawResults, aggregator);

            Console.WriteLine(JsonConvert.SerializeObject(aggregator.BuildResultsObjects(), Formatting.Indented));
            factory.PrintSum();
            int expected = factory.GetSum();
            int actual = (int)aggregator.BuildResults().Data[0][1];
            TimeSpan span = (TimeSpan)aggregator.BuildResults().Data[0][3];
            Console.WriteLine($@"TPS {expected / span.TotalSeconds:N}");
            Console.WriteLine(span.ToString("g"));
            Console.WriteLine();
            Console.WriteLine($@"{expected}/{actual} ? {expected==actual}");
        }
    }
}