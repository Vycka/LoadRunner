using System;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Engine.Strategies.Extensions;

namespace Viki.LoadRunner.Playground
{
    public class BatchStrategyDemo : IScenario
    {
        public static void Run()
        {
            HistogramAggregator aggregator = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(5)))
                .Add(new CountMetric())
                .Add(new TransactionsPerSecMetric());

            StrategyBuilder strategy = new StrategyBuilder()
                .SetScenario<BatchStrategyDemo>()
                .AddSpeed(new BatchingSpeed(TimeSpan.FromSeconds(5), 10))
                .SetThreading(new FixedThreadCount(15))
                .SetLimit(new TimeLimit(TimeSpan.FromSeconds(20)))
                .SetAggregator(aggregator);

            strategy.Build().Run();

            Console.WriteLine(JsonConvert.SerializeObject(aggregator.BuildResultsObjects(), Formatting.Indented));
        }

        public void ScenarioSetup(IIteration context)
        {
        }

        public void IterationSetup(IIteration context)
        {
        }

        public void ExecuteScenario(IIteration context)
        {
            Console.WriteLine(string.Concat(context.ThreadId,"  ", context.GlobalIterationId));
        }

        public void IterationTearDown(IIteration context)
        {
        }

        public void ScenarioTearDown(IIteration context)
        {
        }
    }
}