using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Parameters;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Speed;
using Viki.LoadRunner.Engine.Strategies.Threading;
using Viki.LoadRunner.Tools.Aggregators;
using Viki.LoadRunner.Tools.Windows;

namespace Viki.LoadRunner.Playground
{
    public static class ReadmeDemo
    {
        public static void Run()
        {

            // LoadRunnerParameters used to configure on how load test will execute.
            LoadRunnerParameters loadRunnerParameters = new LoadRunnerParameters
            {
                Limits = new LimitStrategy
                {
                    // Maximum LoadTest duration threshold, after which test is stopped
                    MaxDuration = TimeSpan.FromSeconds(30),

                    // Maximum executed iterations count threshold, after which test is stopped
                    MaxIterationsCount = Int32.MaxValue,

                    // Once LoadTest execution finishes because of [maxDuration] or [maxIterationsCount] limit
                    // coordinating thread will wait [FinishTimeout] amount of time before 
                    // terminating them with Thread.Abort()
                    //
                    // Aborted threads won't get the chance to call IterationTearDown() or ScenarioTearDown()
                    // neither it will broadcast TestContextResultReceived() to aggregators with the state as it is after abort.
                    FinishTimeout = TimeSpan.FromSeconds(60)
                },

                // [ISpeedStrategy] defines maximum allowed load by dampening executed Iterations per second count
                // * Other existing version of [ISpeedStrategy]
                //    - IncremantalSpeed(initialRequestsPerSec: 1.0, increasePeriod: TimeSpan.FromSeconds(10), increaseStep: 3.0)
                Speed = new FixedSpeed(maxIterationsPerSec: Double.MaxValue),

                //[IThreadingStrategy] defines allowed worker thread count
                // * SemiAutoThreading initializes [minThreadCount] at begining
                // It will be increased if not enough threads are available to reach [ISpeedStrategy] limits 
                Threading = new IncrementalThreadCount(15, TimeSpan.FromSeconds(10), 15)
            };

            // Initialize aggregator
            string[] ignoredCheckpoints =
            {
                Checkpoint.IterationSetupCheckpointName,
                Checkpoint.IterationStartCheckpointName,
                Checkpoint.IterationTearDownCheckpointName
            };

            HistogramAggregator histogramAggregator = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(10)))
                .Add(new MinDurationMetric(ignoredCheckpoints))
                .Add(new AvgDurationMetric(ignoredCheckpoints))
                .Add(new MaxDurationMetric(ignoredCheckpoints))
                .Add(new PercentileMetric(new[] {0.5, 0.8, 0.9, 0.95, 0.99}, ignoredCheckpoints))
                .Add(new CountMetric(ignoredCheckpoints))
                .Add(new ErrorCountMetric())
                .Add(new IncrementalThreadCount(15, TimeSpan.FromSeconds(10), 15))
                .Alias($"Min: {Checkpoint.IterationEndCheckpointName}", "Min (ms)")
                .Alias($"Avg: {Checkpoint.IterationEndCheckpointName}", "Avg (ms)")
                .Alias($"Max: {Checkpoint.IterationEndCheckpointName}", "Max (ms)")
                .Alias($"50%: {Checkpoint.IterationEndCheckpointName}", "50% (ms)")
                .Alias($"80%: {Checkpoint.IterationEndCheckpointName}", "80% (ms)")
                .Alias($"90%: {Checkpoint.IterationEndCheckpointName}", "90% (ms)")
                .Alias($"95%: {Checkpoint.IterationEndCheckpointName}", "95% (ms)")
                .Alias($"99%: {Checkpoint.IterationEndCheckpointName}", "99% (ms)")
                .Alias($"Count: {Checkpoint.IterationEndCheckpointName}", "Success: Count")
                .Alias($"Errors: {Checkpoint.IterationSetupCheckpointName}", "Errors: Setup")
                .Alias($"Errors: {Checkpoint.IterationStartCheckpointName}", "Errors: Iteration")
                .Alias($"Errors: {Checkpoint.IterationTearDownCheckpointName}", "Errors: Teardown");
            
            JsonStreamAggregator _jsonStreamAggregator = new JsonStreamAggregator(() => DateTime.Now.ToString("HH_mm_ss__ffff") + ".json");

            //TotalsResultsAggregator resultsAggregator = new TotalsResultsAggregator();

            // Initializing LoadTest Client
            //LoadRunnerEngine loadRunner = LoadRunnerEngine.Create<TestScenario>(loadRunnerParameters, histogramAggregator, _jsonStreamAggregator);

            LoadRunnerUi loadRunnerUi = LoadRunnerUi.Create<TestScenario>(loadRunnerParameters, histogramAggregator, _jsonStreamAggregator);

            Application.Run(loadRunnerUi);
            return;
            // Run test (blocking call)
            //loadRunner.Run();

            //loadRunner.RunAsync();
            //Console.WriteLine("Async started");
            //loadRunner.Wait();
            
            //object defaultResults = histogramAggregator.BuildResultsObjects();
            //Console.WriteLine(JsonConvert.SerializeObject(defaultResults, Formatting.Indented));

            //Console.ReadKey();
        }
    }


    public class TestScenario : ILoadTestScenario
    {
        private static readonly Random Random = new Random(42);

        public void ScenarioSetup(ITestContext testContext)
        {
            Debug.WriteLine("ScenarioSetup Executes on thread creation");
            Debug.WriteLine("Exceptions here are not handled!");
        }

        public void IterationSetup(ITestContext testContext)
        {
            Debug.WriteLine("IterationSetup is executed before each ExecuteScenario call");

            if (Random.Next(100) % 50 == 0)
                throw new Exception("2% error chance for testing");
        }

        public void ExecuteScenario(ITestContext testContext)
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
        

        public void IterationTearDown(ITestContext testContext)
        {
            Debug.WriteLine("IterationTearDown is executed each time after ExecuteScenario iteration is finished.");
            Debug.WriteLine("It is also executed even when IterationSetup or ExecuteScenario fails");

            if (Random.Next(100) % 25 == 0)
                throw new Exception("4% error chance for testing");
        }

        public void ScenarioTearDown(ITestContext testContext)
        {
            Debug.WriteLine("ScenarioTearDown Executes once LoadTest execution is over");

            Debug.WriteLine("Exceptions here are not handled!");
        }
    }
}
/*
[
  {
    "Time (s)": "0",
    "Min (ms)": 0,
    "Avg (ms)": 748,
    "Max (ms)": 1481,
    "50% (ms)": 689,
    "80% (ms)": 1222,
    "90% (ms)": 1360,
    "95% (ms)": 1420,
    "99% (ms)": 1472,
    "Success: Count": 121,
    "Errors: Totals": 24,
    "Errors: Setup": 3,
    "Errors: Iteration": 18,
    "Errors: Teardown": 3
  },
  {
    "Time (s)": "10",
    "Min (ms)": 0,
    "Avg (ms)": 772,
    "Max (ms)": 1487,
    "50% (ms)": 761,
    "80% (ms)": 1222,
    "90% (ms)": 1338,
    "95% (ms)": 1400,
    "99% (ms)": 1482,
    "Success: Count": 118,
    "Errors: Totals": 16,
    "Errors: Setup": 3,
    "Errors: Iteration": 12,
    "Errors: Teardown": 1
  },
  {
    "Time (s)": "20",
    "Min (ms)": 0,
    "Avg (ms)": 764,
    "Max (ms)": 1497,
    "50% (ms)": 713,
    "80% (ms)": 1166,
    "90% (ms)": 1336,
    "95% (ms)": 1432,
    "99% (ms)": 1478,
    "Success: Count": 122,
    "Errors: Totals": 19,
    "Errors: Setup": 2,
    "Errors: Iteration": 12,
    "Errors: Teardown": 5
  }
]
*/
