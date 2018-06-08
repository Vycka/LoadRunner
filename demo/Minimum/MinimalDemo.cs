using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;

namespace LoadRunner.Demo.Minimum
{
    public static class MinimalDemo
    {
        // Define test scenario for a worker thread.
        // new IScenario instance will be created for every worker thread.
        public class MinimalScenario : IScenario
        {
            public void ScenarioSetup(IIteration context)
            {
                // One time setup for each instance/thread
            }

            public void IterationSetup(IIteration context)
            {
                // Setup before each iteration
            }

            public void ExecuteScenario(IIteration context)
            {
                // Only gets called if IterationSetup() succeededs.
                // Timing is meassured here.
            }

            public void IterationTearDown(IIteration context)
            {
                // Cleanup after each iteration (even if IterationSetup() or ExecuteScenario() fails)
            }

            public void ScenarioTearDown(IIteration context)
            {
               // One time cleanup for each instance/thread
            }
        }

        public static void Run()
        {
            // Define how data gets aggregated.
            // Dimensions are like GROUP BY keys in SQL
            // Metrics are aggregation functions like COUNT, SUM, etc..
            HistogramAggregator aggregator = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(5)))
                .Add(new CountMetric())
                .Add(new ErrorCountMetric())
                .Add(new TransactionsPerSecMetric())
                .Add(new PercentileMetric(0.95, 0.99));

            StrategyBuilder strategy = new StrategyBuilder()
                .SetAggregator(aggregator)
                .SetScenario<MinimalScenario>()
                .SetLimit(new TimeLimit(TimeSpan.FromSeconds(20)))
				.SetSpeed(new FixedSpeed(101010))
                .SetThreading(new FixedThreadCount(4));
                
            LoadRunnerEngine engine = strategy.Build();

            engine.Run();

            IEnumerable<object> result = aggregator.BuildResultsObjects();
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            Console.ReadKey();
        }
    }
}