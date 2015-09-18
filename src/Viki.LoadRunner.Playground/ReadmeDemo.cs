using System;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Results;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Parameters;
using Viki.LoadRunner.Engine.Strategies.Speed;
using Viki.LoadRunner.Engine.Strategies.Threading;

namespace Viki.LoadRunner.Playground
{
    public static class ReadmeDemo
    {
        public static void Run()
        {

            // LoadRunnerParameters initializes defaults shown below
            LoadRunnerParameters loadRunnerParameters = new LoadRunnerParameters
            {
                Limits = new ExecutionLimits
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
                SpeedStrategy = new FixedSpeed(maxIterationsPerSec: Double.MaxValue),

                //[IThreadingStrategy] defines allowed worker thread count
                // * SemiAutoThreading initializes [minThreadCount] at begining
                // It will be increased if not enough threads are available to reach [ISpeedStrategy] limits 
                // * Other existing version of [ISpeedStrategy]
                //   - IncrementalThreading(initialThreadcount: 10, increasePeriod: TimeSpan.FromSeconds(10), increaseBatchSize: 5)
                ThreadingStrategy = new SemiAutoThreadCount(minThreadCount: 10, maxThreadCount: 10)
            };

            // Initialize parameters
            LoadRunnerParameters parameters = new LoadRunnerParameters();

            // Initialize aggregator
            TotalsResultsAggregator resultsAggregator = new TotalsResultsAggregator();

            // Initializing LoadTest Client
            LoadRunnerEngine loadRunner = LoadRunnerEngine.Create<TestScenario>(parameters, resultsAggregator);

            // Run test (blocking call)
            loadRunner.Run();

            // ResultItem will have all logged exceptions within LoadTest execution
            ResultsContainer defaultResults = resultsAggregator.GetResults();
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

            //throw new Exception("2% error chance for testing");
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
            Thread.Sleep(Random.Next(4000));

            // [Iteration Begin Checkpoint] -- [First Checkpoint]
            testContext.Checkpoint("First Checkpoint");

            if (Random.Next(100) % 10 == 0)
                throw new Exception("10% error chance for testing");

            // [First Checkpoint] -- [Last Checkpoint]
            testContext.Checkpoint("Last Checkpoint");

            Thread.Sleep(Random.Next(1000));

            if (Random.Next(100) % 100 == 0)
                throw new Exception("1% error chance for testing");
        }
        // [Last Checkpoint] -- [SYS_ITERATION_END]

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