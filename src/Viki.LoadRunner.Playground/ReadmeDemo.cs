using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
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

            LoadRunnerParameters loadRunnerParameters = new LoadRunnerParameters
            {
                Limits = new LimitStrategy
                {
                    MaxDuration = TimeSpan.FromSeconds(20),

                    MaxIterationsCount = Int32.MaxValue,

                    FinishTimeout = TimeSpan.FromSeconds(60)
                },

                Speed = new ISpeedStrategy[] { new MaxSpeed() },
                //Speed = new ListOfSpeed(TimeSpan.FromSeconds(60), 5, 1, Double.MaxValue),

                Threading = new ListOfCounts(TimeSpan.FromSeconds(10), 4, 20)
            };

            // Initialize aggregator
            string[] ignoredCheckpoints =
            {
                Checkpoint.Names.Setup,
                Checkpoint.Names.IterationStart,
                Checkpoint.Names.TearDown
            };

            HistogramAggregator histogramAggregator = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(10)))
                .Add(new MinDurationMetric(ignoredCheckpoints))
                .Add(new AvgDurationMetric(ignoredCheckpoints))
                .Add(new MaxDurationMetric(ignoredCheckpoints))
                .Add(new PercentileMetric(new[] {0.95}, ignoredCheckpoints))
                .Add(new CountMetric(ignoredCheckpoints))
                .Add(new TransactionsPerSecMetric())
                .Add(new ErrorCountMetric())
                .Alias($"Min: {Checkpoint.Names.IterationEnd}", "Min (ms)")
                .Alias($"Avg: {Checkpoint.Names.IterationEnd}", "Avg (ms)")
                .Alias($"Max: {Checkpoint.Names.IterationEnd}", "Max (ms)")
                .Alias($"50%: {Checkpoint.Names.IterationEnd}", "50% (ms)")
                .Alias($"80%: {Checkpoint.Names.IterationEnd}", "80% (ms)")
                .Alias($"90%: {Checkpoint.Names.IterationEnd}", "90% (ms)")
                .Alias($"95%: {Checkpoint.Names.IterationEnd}", "95% (ms)")
                .Alias($"99%: {Checkpoint.Names.IterationEnd}", "99% (ms)")
                .Alias($"Count: {Checkpoint.Names.IterationEnd}", "Success: Count")
                .Alias($"Errors: {Checkpoint.Names.Setup}", "Errors: Setup")
                .Alias($"Errors: {Checkpoint.Names.IterationStart}", "Errors: Iteration")
                .Alias($"Errors: {Checkpoint.Names.TearDown}", "Errors: Teardown");

            JsonStreamAggregator _jsonStreamAggregator =
                new JsonStreamAggregator(() => DateTime.Now.ToString("HH_mm_ss__ffff") + ".json");

            //TotalsResultsAggregator resultsAggregator = new TotalsResultsAggregator();

            // Initializing LoadTest Client
            //LoadRunnerEngine loadRunner = LoadRunnerEngine.Create<TestScenario>(loadRunnerParameters, histogramAggregator, _jsonStreamAggregator);

            LoadRunnerUi loadRunnerUi = LoadRunnerUi.Create<BlankScenario>(loadRunnerParameters, histogramAggregator);

            Application.Run(loadRunnerUi);

            // Run test (blocking call)
            //loadRunner.Run();

            //loadRunner.RunAsync();
            //Console.WriteLine("Async started");
            //loadRunner.Wait();

            object defaultResults = histogramAggregator.BuildResultsObjects();
            Console.WriteLine(JsonConvert.SerializeObject(defaultResults, Formatting.Indented));

            Console.ReadKey();
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
