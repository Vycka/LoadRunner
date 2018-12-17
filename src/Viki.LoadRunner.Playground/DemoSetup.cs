using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Engine.Strategies.Extensions;

namespace Viki.LoadRunner.Playground
{
    public static class DemoSetup
    {
        public static void Run()
        {

            // Initialize aggregator
            string[] ignoredCheckpoints =
            {
                Checkpoint.Names.Setup,
                Checkpoint.Names.TearDown
            };

            HistogramAggregator histogramAggregator = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(10)))
                .Add(new FuncMetric<TimeSpan>("TMin", TimeSpan.MaxValue,
                    (span, result) => span > result.IterationStarted ? result.IterationStarted : span))
                .Add(new FuncMetric<TimeSpan>("TMax", TimeSpan.MinValue,
                    (span, result) => span < result.IterationFinished ? result.IterationFinished : span))
                .Add(new FuncMetric<int>("Working Threads", 0,
                    (i, result) => result.CreatedThreads - result.IdleThreads))
                //.Add(new MinDurationMetric(ignoredCheckpoints))
                .Add(new AvgDurationMetric(ignoredCheckpoints))
                .Add(new MaxDurationMetric(ignoredCheckpoints))
                //.Add(new PercentileMetric(new[] {0.99999}, ignoredCheckpoints))
                .Add(new CountMetric(ignoredCheckpoints))
                .Add(new TransactionsPerSecMetric())
                .Add(new ErrorCountMetric())
                .Alias($"Min: {Checkpoint.Names.Iteration}", "Min (ms)")
                .Alias($"Avg: {Checkpoint.Names.Iteration}", "Avg (ms)")
                .Alias($"Max: {Checkpoint.Names.Iteration}", "Max (ms)")
                .Alias($"50%: {Checkpoint.Names.Iteration}", "50% (ms)")
                .Alias($"80%: {Checkpoint.Names.Iteration}", "80% (ms)")
                .Alias($"90%: {Checkpoint.Names.Iteration}", "90% (ms)")
                .Alias($"95%: {Checkpoint.Names.Iteration}", "95% (ms)")
                .Alias($"99.99%: {Checkpoint.Names.Iteration}", "99.99% (ms)")
                .Alias($"Count: {Checkpoint.Names.Iteration}", "Success: Count");


            StrategyBuilder strategy = new StrategyBuilder()
                .SetScenario<TestScenario>()
                .SetLimit(new TimeLimit(TimeSpan.FromSeconds(10)))
                .SetThreading(new FixedThreadCount(40))
                .SetSpeed(new FixedSpeed(2000)) // Tps is lower in results due to failed iterations not being counted
                .SetFinishTimeout(TimeSpan.FromSeconds(60))
                .SetAggregator(histogramAggregator);


            IStrategyExecutor engine = strategy.Build();
            engine.Run();


            object defaultResults = histogramAggregator.BuildResultsObjects();
            Console.WriteLine(JsonConvert.SerializeObject(defaultResults, Formatting.Indented));
        }
    }


    public class TestScenario : IScenario
    {
        private static readonly Random Random = new Random(42);

        public void ScenarioSetup(IIteration context)
        {
            // ScenarioSetup Executes on thread creation
            // Exceptions here are not handled!
        }

        public void IterationSetup(IIteration context)
        {
            // IterationSetup is executed before each ExecuteScenario call

            if (Random.Next(100) % 50 == 0)
                throw new Exception("2% error chance for testing");
        }

        public void ExecuteScenario(IIteration context)
        {
            
            // ExecuteScenario defines single iteration for load test scenario
            // It is called after each successful IterationSetup call.
            // Execution time is measured only for this function
            // You can use testContext.Checkpoint() function to mark points,
            // where time should be measured

            Thread.Sleep(Random.Next(1500));

            if (Random.Next(100) % 10 == 0)
                throw new Exception("10% error chance for testing");
        }


        public void IterationTearDown(IIteration context)
        {
            // IterationTearDown is executed each time after ExecuteScenario iteration is finished.
            // It is also executed even when IterationSetup or ExecuteScenario fails

            if (Random.Next(100) % 25 == 0)
                throw new Exception("4% error chance for testing");
        }

        public void ScenarioTearDown(IIteration context)
        {
            // "ScenarioTearDown Executes once LoadTest execution is over
            //"Exceptions here are not handled!
        }
    }
}
