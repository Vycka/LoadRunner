using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Tools.Aggregators;

namespace LoadRunner.Demo.ReadmeDemo
{
    public static class BareMinimum
    {
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
                // Meassured iteration execution if IterationSetup() succeeded
            }

            public void IterationTearDown(IIteration context)
            {
                // Iteration cleanup (even if IterationSetup() or ExecuteScenario() fails)
            }

            public void ScenarioTearDown(IIteration context)
            {
               // One time cleanup for each instance/thread
            }
        }

        public static void Run()
        {
            // Output for json masterdata
            IAggregator jsonAggregator = new JsonStreamAggregator("json.masterdata.txt");

            // Custom aggregation
            HistogramAggregator aggregator = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(5)))
                .Add(new CountMetric())
                .Add(new ErrorCountMetric())
                .Add(new PercentileMetric(0.95, 0.99));

            StrategyBuilder strategy = new StrategyBuilder()
                .SetAggregator(aggregator, jsonAggregator)
                .SetScenario<MinimalScenario>()
                .SetLimit(new TimeLimit(TimeSpan.FromSeconds(20)))
                .SetThreading(new FixedThreadCount(4))
                .SetSpeed(new FixedSpeed(100000));

            LoadRunnerEngine engine = strategy.Build();

            engine.Run();

            IEnumerable<object> result = aggregator.BuildResultsObjects();
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            Console.ReadKey();
        }
    }
}