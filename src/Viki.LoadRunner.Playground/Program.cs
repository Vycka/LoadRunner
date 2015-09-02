using System;
using System.Threading;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Aggregates.Default;
using Viki.LoadRunner.Engine.Executor;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            DefaultResultsAggregator resultsAggregator = new DefaultResultsAggregator();

            LoadTestClient testClient = 
                LoadTestClient.Create<TestScenario>(
                    new ExecutionParameters(
                        maxDuration: TimeSpan.FromSeconds(3),
                        minThreads: 10,
                        maxThreads: 100,
                        maxRequestsPerSecond: 2,
                        finishTimeoutMilliseconds: 4000,
                        maxIterationsCount: Int32.MaxValue
                    ),
                    resultsAggregator
                );

            testClient.Run();

            var results = resultsAggregator.BuildResultsObject();

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
        }
    }

    public class TestScenario : ITestScenario
    {
        private static readonly Random Random = new Random(42);

        public void ScenarioSetup(ITestContext testContext)
        {
            Console.WriteLine($"ScenarioSetup {testContext.ThreadId}");
        }

        public void ScenarioTearDown(ITestContext testContext)
        {
            
            Console.WriteLine($"ScenarioTearDown {testContext.ThreadId}");
        }

        public void IterationSetup(ITestContext testContext)
        {
            //Console.WriteLine($"IterationSetup {testContext.ThreadId} {testContext.IterartionId}");
        }

        public void IterationTearDown(ITestContext testContext)
        {
            //Console.WriteLine($"IterationTearDown {testContext.ThreadId} {testContext.IterartionId}");
        }

        public void ExecuteScenario(ITestContext testContext)
        {
           // testContext.Checkpoint();

            Thread.Sleep(Random.Next(1));

            //testContext.Checkpoint("Checkpoint AAA");

            //Thread.Sleep(Random.Next(1000));

            //testContext.Checkpoint("Checkpoint BBB");

            //Thread.Sleep(Random.Next(1000));
        }
    }
}
