﻿using System;
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

namespace LoadRunner.Demo.Guides.QuickStart
{
    // [1] Scenario:
    // Define test scenario for a worker thread.
    // new IScenario instance will be created for every worker thread.
    //
    // no async/await code allowed, as that will break execution due to how it behaves with void functions
    //   task.Result is only workaround
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
            // Time is measured only here.

            // Only gets called if IterationSetup() succeeds.

            // Will be skipped if graceful stop is triggered before the call.
        }

        public void IterationTearDown(IIteration context)
        {
            // Cleanup after each iteration (even if IterationSetup() or ExecuteScenario() fails)
            
            // Will be skipped if ExecuteScenario() was skipped because graceful stop.
        }

        public void ScenarioTearDown(IIteration context)
        {
            // One time cleanup for each instance/thread
        }
    }

    public static class QuickStartDemo
    {
        public static void Run()
        {
            // [2] Results aggregation (Or raw measurement collection, see RawDataMeasurementsDemo.cs)
            // Define how data gets aggregated.
            // Dimensions are like GROUP BY keys in SQL
            // Metrics are aggregation functions like COUNT, SUM, etc..
            // Extensive HistogramAggregator demo now WiP
            HistogramAggregator aggregator = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(5)))
                .Add(new CountMetric())
                .Add(new ErrorCountMetric())
                .Add(new TransactionsPerSecMetric())
                .Add(new PercentileMetric(0.95, 0.99));

            // Secondary aggregation just to monitor key metrics.
            KpiPrinterAggregator kpiPrinter = new KpiPrinterAggregator(
                TimeSpan.FromSeconds(5),
                new CountMetric(Checkpoint.NotMeassuredCheckpoints),
                new ErrorCountMetric(false),
                new TransactionsPerSecMetric()
            );

            // [3] Execution settings
            // Using StrategyBuilder put defined aggregation, scenario, and execution settings into one object
            StrategyBuilder strategy = new StrategyBuilder()
                .SetAggregator(aggregator, kpiPrinter) // Optional
                .SetScenario<BlankScenario>() // Required
                .SetLimit(new TimeLimit(TimeSpan.FromSeconds(20))) // Optional, but if not provided, execution will never stop - unless running test with RunAsync() and stopping later with CancelAsync(true)
                .SetSpeed(new FixedSpeed(100000)) // Optional (Skip for maximum throughput)
                .SetThreading(new FixedThreadCount(4)); // Required



            // [4] Execution
            // All that's left is Build(), run and wait for completion and print out measured results.
            LoadRunnerEngine engine = strategy.Build();
            engine.Run();

            IEnumerable<object> result = aggregator.BuildResultsObjects();
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}