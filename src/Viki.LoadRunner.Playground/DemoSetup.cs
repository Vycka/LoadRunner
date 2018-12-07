using System;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Engine.Strategies.Extensions;
using Viki.LoadRunner.Engine.Validators;
using Viki.LoadRunner.Tools.Aggregators;
using Viki.LoadRunner.Tools.Extensions;

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
                    (i, result) => result.CreatedThreads + result.IdleThreads))
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
                .SetSpeed(new FixedSpeed(7.42)) // Tps is lower in results due to failed iterations not being counted
                .SetFinishTimeout(TimeSpan.FromSeconds(60))
                .SetAggregator(new JsonStreamAggregator("wat.txt"));


            IStrategyExecutor engine = strategy.BuildUi(new ScenarioValidator(strategy.ScenarioFactory));
            engine.Run();

            JsonStreamAggregator.Replay("wat.txt", histogramAggregator);

            object defaultResults = histogramAggregator.BuildResultsObjects();
            Console.WriteLine(JsonConvert.SerializeObject(defaultResults, Formatting.Indented));

            Console.ReadKey();
        }
    }


    public class TestScenario : IScenario
    {
        private static readonly Random Random = new Random(42);

        public void ScenarioSetup(IIteration context)
        {
            Debug.WriteLine("ScenarioSetup Executes on thread creation");
            Debug.WriteLine("Exceptions here are not handled!");
        }

        public void IterationSetup(IIteration context)
        {
            Debug.WriteLine("IterationSetup is executed before each ExecuteScenario call");

            if (Random.Next(100) % 50 == 0)
                throw new Exception("2% error chance for testing");
        }

        public void ExecuteScenario(IIteration context)
        {
            Debug.WriteLine(
                "ExecuteScenario defines single iteration for load test scenario, " +
                "It is called after each successful IterationSetup call. " +
                "Execution time is measured only for this function" +
                "You can use testContext.Checkpoint() function to mark points, " +
                "where time should be measured"
            );

            Thread.Sleep(Random.Next(1500));

            if (Random.Next(100) % 10 == 0)
                throw new Exception("10% error chance for testing");
        }


        public void IterationTearDown(IIteration context)
        {
            Debug.WriteLine("IterationTearDown is executed each time after ExecuteScenario iteration is finished.");
            Debug.WriteLine("It is also executed even when IterationSetup or ExecuteScenario fails");

            if (Random.Next(100) % 25 == 0)
                throw new Exception("4% error chance for testing");
        }

        public void ScenarioTearDown(IIteration context)
        {
            Debug.WriteLine("ScenarioTearDown Executes once LoadTest execution is over");

            Debug.WriteLine("Exceptions here are not handled!");
        }
    }
}

/*
 

Speed = new ISpeedStrategy[] { new MaxSpeed() },
 

DEBUG,  ATTACHED

[
  {
    "TMin": "00:00:00.0029506",
    "TMax": "00:00:10.0996174",
    "Avg (ms)": 0,
    "Max (ms)": 169,
    "99,999%: ITERATION_END": 47,
    "Success: Count": 3893384,
    "TPS": 385610.82356406969,
    "Errors: Totals": 0
  }
]

RELEASE, NOT ATTACHED

[
  {
    "TMin": "00:00:00.0037797",
    "TMax": "00:00:10.0053868",
    "Avg (ms)": 0,
    "Max (ms)": 94,
    "99,999%: ITERATION_END": 14,
    "Success: Count": 5273961,
    "TPS": 527311.35579201067,
    "Errors: Totals": 0
  }
]
 
*/
