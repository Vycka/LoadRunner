using System;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Engine.Strategies.Extensions;

namespace LoadRunner.Demo.Theoretical
{
    public class AggregationImpactDemo
    {
        
        // While this test can achieve 1million/sec speed, current implementation won't be able to process this massive amount of incomming data
        // as result of it, data gets queued up in memory and this test will easily eat ~5 GB's of ram in those 13 seconds of execution so be sure that there is enough of RAM available before running this one.
        //
        // This also shows how Run() won't exit past 13seconds. While test is no longer running, engine waits until all data gets processed.
        public static void Run()
        {
            HistogramAggregator aggregator = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(4)))
                .Add(new CountMetric())
                .Add(new TransactionsPerSecMetric());

            //var kpiAggregator = new KpiPrinterAggregator(
            //    TimeSpan.FromSeconds(1),
            //    new MaxDurationMetric(),
            //    new CountMetric(Checkpoint.NotMeassuredCheckpoints),
            //    new TransactionsPerSecMetric());

            StrategyBuilder strategy = new StrategyBuilder()
                .SetScenario<BlankScenario>()
                .SetThreading(new FixedThreadCount(4))
                .SetLimit(new TimeLimit(TimeSpan.FromSeconds(13)))
                .SetAggregator(aggregator);

            strategy.Build().Run();

            Console.WriteLine(JsonConvert.SerializeObject(aggregator.BuildResultsObjects(), Formatting.Indented));
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