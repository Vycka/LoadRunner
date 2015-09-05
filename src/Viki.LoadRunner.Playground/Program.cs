using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Results;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Parameters;
using Viki.LoadRunner.Engine.Strategies.Threading;
using Viki.LoadRunner.Engine.Utils;

namespace Viki.LoadRunner.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            //ReadmeDemo.Run();

            //return;
            DefaultResultsAggregator defaultResultsAggregator = new DefaultResultsAggregator();
            HistogramResultsAggregator histogramResultsAggregator = new HistogramResultsAggregator(aggregationStepSeconds: 2);


            LoadRunnerEngine testClient =
                LoadRunnerEngine.Create<LoadTestScenario>(
                    new LoadRunnerParameters(
                        maxDuration: TimeSpan.FromSeconds(30),
                        maxRequestsPerSecond: Double.MaxValue,
                        finishTimeoutMilliseconds: 10000,
                        maxIterationsCount: int.MaxValue
                        )
                    {
                        ThreadingStrategy = new IncrementalThreading(TimeSpan.FromSeconds(5), 10, 10)
                    },
                    defaultResultsAggregator, histogramResultsAggregator
                );
                

            testClient.Run();


            ResultsContainer results = defaultResultsAggregator.GetResults();
            List<HistogramResultRow> histogramResults = histogramResultsAggregator.GetResults().ToList();

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
            HistogramCsvExport.Export(histogramResults, "d:\\exportTest.csv");
        }
    }

    public class LoadTestScenario : ILoadTestScenario
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
            if (Random.Next(100) % 100 == 0)
                throw new Exception($"#### {testContext.IterartionId}");
        }

        public void IterationTearDown(ITestContext testContext)
        {
            if (Random.Next(100) % 50 == 0)
                throw new Exception($"#### {testContext.IterartionId}");
            //Console.WriteLine($"IterationTearDown {testContext.ThreadId} {testContext.IterartionId}");
        }

        public void ExecuteScenario(ITestContext testContext)
        {
            if (Random.Next(100) % 10 == 0)
                throw new Exception($"@@@@ {testContext.IterartionId}");

            Thread.Sleep(Random.Next(700));
            
            testContext.Checkpoint("Checkpoint AAA");


            if (Random.Next(100) % 50 == 0)
                throw new Exception("err");

            Thread.Sleep(Random.Next(1000));
        }
    }
}
