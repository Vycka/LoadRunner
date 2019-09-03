using System;
using System.Diagnostics;
using System.Linq;
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
        public static void Run()
        {
            HistogramAggregator aggregator = new HistogramAggregator()
                .Add(new CountMetric())
                .Add(new FuncMetric<TimeSpan>("max", TimeSpan.Zero, (s, result) => s < result.IterationStarted ? result.IterationStarted : s));

            AssertIterationIdsAggregator idsValidator = new AssertIterationIdsAggregator();

            int streamCount = 0;
            StreamAggregator streamAggregator = new StreamAggregator(results => streamCount = results.Count());

            CountingScenarioFactory factory = new CountingScenarioFactory();

            StrategyBuilder strategy = new StrategyBuilder()
                .SetScenario(factory)
                .SetThreading(new FixedThreadCount(16))
                .SetLimit(new TimeLimit(TimeSpan.FromSeconds(12)))
                .SetAggregator(aggregator, idsValidator, streamAggregator);

           
            Stopwatch sw = new Stopwatch();
            sw.Start();
            strategy.Build().Run();
            sw.Stop();

            Console.WriteLine(JsonConvert.SerializeObject(aggregator.BuildResultsObjects(), Formatting.Indented));
            factory.PrintSum();
            int expected = factory.GetSum();
            int actual = (int)aggregator.BuildResults().Data[0][1];
            TimeSpan lastIteration = (TimeSpan)aggregator.BuildResults().Data[0][3];
            Console.WriteLine($@"TPS {expected / lastIteration.TotalSeconds:N}");
            Console.WriteLine($@"Last iteration ended at: {lastIteration:g}");
            Console.WriteLine($@"Aggregator catchup took: {(sw.Elapsed-lastIteration):g}");
            Console.WriteLine();
            Console.WriteLine($@"{expected}/{actual} & {streamCount} ? {expected==actual} && {expected==streamCount}");
            Console.WriteLine();
            idsValidator.PrintResults();
        }
    }
}