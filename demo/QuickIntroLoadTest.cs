using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Parameters;
using Viki.LoadRunner.Engine.Strategies.Speed;
using Viki.LoadRunner.Engine.Strategies.Threading;

namespace LoadRunner.Demo
{
    public class QuickIntroLoadTest
    {
        private static readonly string[] IgnoredCheckpoints =
        {
                Checkpoint.IterationSetupCheckpointName,
                Checkpoint.IterationStartCheckpointName,
                Checkpoint.IterationTearDownCheckpointName
        };

        // This demonstrates default parameters of LoadRunnerParameters object
        private readonly LoadRunnerParameters loadRunnerParameters = new LoadRunnerParameters
        {
            Limits = new ExecutionLimits
            {
                // Maximum LoadTest duration threshold, after which test is stopped
                MaxDuration = TimeSpan.FromSeconds(30),

                // Maximum executet iterations count threshold, after which test is stopped
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
            SpeedStrategy = new FixedSpeed(maxIterationsPerSec: Double.MaxValue),

            // [IThreadingStrategy] controls allowed worker thread count
            // More info https://github.com/Vycka/LoadRunner/wiki/IThreadingStrategy
            ThreadingStrategy = new SemiAutoThreadCount(minThreadCount: 10, maxThreadCount: 10)
        };
        

        public static void Run()
        {
            // For this example we will create a little bit modified version of it
            LoadRunnerParameters parameters = new LoadRunnerParameters
            {
                // Incremental strategy here increases thread count every 10 seconds.
                // This can be aligned with TimeDimension used below in shown aggregator.
                ThreadingStrategy = new IncrementalThreadCount(20, TimeSpan.FromSeconds(10), 20)
            };

            // Initialize aggregators you want to use. 
            // This preset shown below more or less should work for most of the test cases
            // You might want to adjust TimeDimension to aggregate totals in few time chunks 
            //
            // HistogramAggregator is a modular Dimension/Metric style aggregator tool, which can be expanded by implementing IMetric or IDimension
            //
            // Dimensions are like row keys (Adding multiple dimensions would multiply result row count)
            // * Currently only TimeDimension and FuncDimension is available
            // Metrics are like values, which will be meassured in test execution
            // * Most usable metrics are all defined below
            HistogramAggregator histogramAggregator = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(10)))
                .Add(new MinDurationMetric(IgnoredCheckpoints))
                .Add(new AvgDurationMetric(IgnoredCheckpoints))
                .Add(new MaxDurationMetric(IgnoredCheckpoints))
                .Add(new PercentileMetric(new[] { 0.5, 0.8, 0.9, 0.95, 0.99 }, IgnoredCheckpoints))
                .Add(new CountMetric(IgnoredCheckpoints))
                .Add(new ErrorCountMetric())
                .Add(new FuncMetric<int>("Created Threads", 0, (i, result) => result.CreatedThreads))
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

            // This demo shows currently this modular HistogramAggregator undocumented aggregator
            // Though there are older ones "Legacy" which are somewhat documented
            // https://github.com/Vycka/LoadRunner/wiki/IResultsAggregator but avoid using them until HistogramAggregator is not enough (e.g. if wanting to make Exceptions dump)

            // Now we initialize LoadRunnerEngine by providing:
            // * Type of class which implements ILoadTestScenario (e.g DemoTestScenario)
            // * LoadRunnerParameters
            // * As many aggregators as you like 
            LoadRunnerEngine loadRunner = LoadRunnerEngine.Create<DemoTestScenario>(parameters, histogramAggregator);

            // Run test (blocking call)
            loadRunner.Run();

            // Once finished, extract information from used aggregators, and do some exceling :)
            // BuildResultsObjects() will produce results in structure compatible with JSON -> CSV converters. (See ~20 lines below)
            IEnumerable<object> defaultResults = histogramAggregator.BuildResultsObjects();

            //Alternative export way is 
            // HistogramResults results = histogramAggregator.BuildResults();
            //
            // results will be presented in this structure:
            // * public readonly string[] ColumnNames;
            // * public readonly object[][] Values;


            Console.WriteLine(JsonConvert.SerializeObject(defaultResults, Formatting.Indented));
            Console.ReadKey();
        }
    }
}

// BuildResultsObjects() results
// You will get result similar to this.
// This structure can be used with online JSON -> CSV converters.
// Imported to excel, and some charts could be drawn with it.
// See DemoResults.xlsx 
/*
[
  {
    "Time (s)": "0",
    "Min (ms)": 30,
    "Avg (ms)": 483,
    "Max (ms)": 964,
    "50% (ms)": 497,
    "80% (ms)": 733,
    "90% (ms)": 800,
    "95% (ms)": 853,
    "99% (ms)": 921,
    "Success: Count": 363,
    "Errors: Totals": 61,
    "Errors: Iteration": 38,
    "Errors: Teardown": 12,
    "Errors: Setup": 11,
    "Created Threads": 20
  },
  {
    "Time (s)": "10",
    "Min (ms)": 6,
    "Avg (ms)": 512,
    "Max (ms)": 994,
    "50% (ms)": 508,
    "80% (ms)": 759,
    "90% (ms)": 834,
    "95% (ms)": 905,
    "99% (ms)": 958,
    "Success: Count": 688,
    "Errors: Totals": 123,
    "Errors: Iteration": 73,
    "Errors: Teardown": 30,
    "Errors: Setup": 20,
    "Created Threads": 40
  },
  {
    "Time (s)": "20",
    "Min (ms)": 13,
    "Avg (ms)": 499,
    "Max (ms)": 980,
    "50% (ms)": 499,
    "80% (ms)": 743,
    "90% (ms)": 818,
    "95% (ms)": 874,
    "99% (ms)": 924,
    "Success: Count": 1052,
    "Errors: Totals": 202,
    "Errors: Iteration": 134,
    "Errors: Teardown": 46,
    "Errors: Setup": 22,
    "Created Threads": 60
  },
  {
    "Time (s)": "30",
    "Min (ms)": 110,
    "Avg (ms)": 577,
    "Max (ms)": 961,
    "50% (ms)": 607,
    "80% (ms)": 806,
    "90% (ms)": 888,
    "95% (ms)": 912,
    "99% (ms)": 957,
    "Success: Count": 54,
    "Errors: Totals": 8,
    "Errors: Iteration": 6,
    "Errors: Teardown": 2,
    "Errors: Setup": null,
    "Created Threads": 80
  }
]*/
