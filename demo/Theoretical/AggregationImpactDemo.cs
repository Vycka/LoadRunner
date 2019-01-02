using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Engine.Strategies.Extensions;

namespace LoadRunner.Demo.Theoretical
{
    public class AggregationImpactDemo
    {

        // Registering at least one aggregation forces worker threads to copy/produce measurements after each iteration.
        // Registering many aggregations shouldn't impact test performance more compared to just a single aggregation.
        //
        //
        // While this test can achieve 1+million/sec speed (w/o debugger and release build):
        // current HistogramAggregator implementation won't be able to process this massive amount of incoming data in real time.
        // as result of it, data gets queued up in memory and this test will easily eat ~5+ GB's of ram in first test run,
        // so be sure that there is enough of RAM available before running this one.
        //
        // This also shows how Run() won't exit past 13seconds. While test is no longer running, engine waits until all data gets processed.
        // 
        // This example runs test twice:
        //  * One with simple histogram setup 
        //  * And other without histogram but receiving a direct stream of raw measurements and just counting them.
        //    - This should be able to count results in the real time
        public static void Run()
        {
            TimeSpan duration = TimeSpan.FromSeconds(13); // 2x tests

            HistogramAggregator aggregator = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(4)))
                .Add(new CountMetric())
                .Add(new TransactionsPerSecMetric());

            StrategyBuilder strategy = new StrategyBuilder()
                .SetScenario<BlankScenario>()
                .SetThreading(new FixedThreadCount(4))
                .SetLimit(new TimeLimit(duration))
                .SetAggregator(aggregator);

            TimeSpan durationWithHistogram = MeassuredRun(strategy.Build());

            // Ease use of ram ASAP.
            GC.Collect(1, GCCollectionMode.Forced, true);

            int iterations = 0;
            strategy.SetAggregator(new StreamAggregator(result => iterations = iterations + 1));

            TimeSpan durationWithStream = MeassuredRun(strategy.Build());

            Console.WriteLine(JsonConvert.SerializeObject(aggregator.BuildResultsObjects(), Formatting.Indented));
            Console.WriteLine($"Time took for [HistogramAggregator] to process whats left in the buffer: {durationWithHistogram - duration:g}");
            Console.WriteLine($"Time took for [StreamAggregator] to count whats left in the buffer: {durationWithStream - duration:g}");

        }

        private static TimeSpan MeassuredRun(IStrategyExecutor executor)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            executor.Run();
            sw.Stop();

            return sw.Elapsed;
        }
    }

    public class BlankScenario : IScenario
    {
        public void ScenarioSetup(IIteration context)
        {
        }

        public void IterationSetup(IIteration context)
        {
        }

        public void ExecuteScenario(IIteration context)
        {
        }

        public void IterationTearDown(IIteration context)
        {

        }

        public void ScenarioTearDown(IIteration context)
        {
        }
    }
}
