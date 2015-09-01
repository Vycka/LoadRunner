using System;
using System.Threading;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Executor;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            LoadRunner<TestScenario> testRunner = 
                new LoadRunner<TestScenario>(
                    new ExecutionParameters(
                        maxDuration: TimeSpan.FromSeconds(10),
                        minThreads: 1,
                        maxThreads: 200,
                        maxRequestsPerSecond: Int32.MaxValue,
                        finishTimeoutMilliseconds: 4000
                    )
                );

            testRunner.Run();

            var results = testRunner.QueryResults();

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
            Console.WriteLine($"IterationSetup {testContext.ThreadId} {testContext.IterartionId}");
        }

        public void IterationTearDown(ITestContext testContext)
        {
            Console.WriteLine($"IterationTearDown {testContext.ThreadId} {testContext.IterartionId}");
        }

        public void ExecuteScenario(ITestContext testContext)
        {
            testContext.Checkpoint();

            Thread.Sleep(Random.Next(1000));

            testContext.Checkpoint("Checkpoint AAA");

            Thread.Sleep(Random.Next(1000));

            testContext.Checkpoint("Checkpoint BBB");

            Thread.Sleep(Random.Next(1000));
        }
    }
}
