using System;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Results;
using Viki.LoadRunner.Engine.Executor;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Playground
{
    public static class ReadmeDemo
    {
        public static void Run()
        {
            ExecutionParameters executionParameters = new ExecutionParameters(
                maxDuration: TimeSpan.FromSeconds(15),
                maxIterationsCount: 5000,
                minThreads: 1,
                maxThreads: 1,
                maxRequestsPerSecond: Double.MaxValue,
                finishTimeoutMilliseconds: 10000
            );

            DefaultResultsAggregator resultsAggregator = new DefaultResultsAggregator();

            // Initializing LoadTest Client
            LoadRunnerEngine loadRunner = LoadRunnerEngine.Create<TestScenario>(executionParameters, resultsAggregator);

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
            Thread.Sleep(Random.Next(5000));

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
            throw new Exception("2% error chance for testing");
        }
    }
}