using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Engine.Strategies.Extensions;
using Viki.LoadRunner.Tools.ConsoleUi;

namespace LoadRunner.Demo.Minimum
{
    // [1] Scenario:
    // Define test scenario for a worker thread.
    // new IScenario instance will be created for every worker thread.
    public class BlankScenario : IScenario
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

    public static class MinimalDemo
    {
        public static void Run()
        {
            // [2] Results aggregation (Or raw meassurement collection, see RawResults)
            // Define how data gets aggregated.
            // Dimensions are like GROUP BY keys in SQL
            // Metrics are aggregation functions like COUNT, SUM, etc..
            HistogramAggregator aggregator = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(5)))
                .Add(new CountMetric())
                .Add(new ErrorCountMetric())
                .Add(new TransactionsPerSecMetric())
                .Add(new PercentileMetric(0.95, 0.99));

            // Optional secondary aggregation just to monitor key metrics (avoid cpu intensive metrics here if test is cpu intensive)
            KpiPrinterAggregator kpiPrinter = new KpiPrinterAggregator(
                TimeSpan.FromSeconds(5),
                new PercentileMetric(0.95, 1), // Avoid using this metric if not sure about cpu capabilities.
                new MaxDurationMetric(),
                new CountMetric(Checkpoint.NotMeassuredCheckpoints),
                new ErrorCountMetric(false),
                new TransactionsPerSecMetric()
            );


            // [3] Execution settings
            // Using StrategyBuilder put defined aggregation, scenario, and execution settings into one object
            StrategyBuilder strategy = new StrategyBuilder()
                .SetAggregator(aggregator, kpiPrinter) // Optional
                .SetScenario<BlankScenario>() // Required
                .SetLimit(new TimeLimit(TimeSpan.FromSeconds(20))) // Optional, but if not provided, execution will never stop. one has to trigger it in engine.
				.SetSpeed(new FixedSpeed(100000)) // Optional
                .SetThreading(new FixedThreadCount(4)); // Required



            // [4] Execution
            // All thats left is Build(), run and wait for completion and print out meassured results.
            LoadRunnerEngine engine = strategy.Build();
            engine.Run();

            IEnumerable<object> result = aggregator.BuildResultsObjects();
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}