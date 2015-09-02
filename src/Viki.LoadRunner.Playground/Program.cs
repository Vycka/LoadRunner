using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Aggregates;
using Viki.LoadRunner.Engine.Aggregates.Utils;
using Viki.LoadRunner.Engine.Executor;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            DefaultResultsAggregator defaultResultsAggregator = new DefaultResultsAggregator();
            HistogramResultsAggregator histogramResultsAggregator = new HistogramResultsAggregator(3);

            LoadTestClient testClient = 
                LoadTestClient.Create<TestScenario>(
                    new ExecutionParameters(
                        maxDuration: TimeSpan.FromSeconds(5),
                        minThreads: 10,
                        maxThreads: 100,
                        maxRequestsPerSecond: 100,
                        finishTimeoutMilliseconds: 4000,
                        maxIterationsCount: Int32.MaxValue
                    ),
                    defaultResultsAggregator,
                    histogramResultsAggregator
                );

            testClient.Run();

            List<ResultItem> defaultResults = defaultResultsAggregator.GetResults().ToList();
            List<HistogramResultRow> histogramResults = histogramResultsAggregator.GetResults().ToList();

            Console.WriteLine(JsonConvert.SerializeObject(defaultResults, Formatting.Indented));
            Console.WriteLine(JsonConvert.SerializeObject(histogramResults, Formatting.Indented));
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
