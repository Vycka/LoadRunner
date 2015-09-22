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
        static void Main()
        {
            //ReadmeDemo.Run();

            string[] nameAggregates = new[] {"AAA", "BBB", "CCC"};

            TestContextResult validatorResult = LoadTestScenarioValidator.Validate(new LoadTestScenario());
            Console.WriteLine(JsonConvert.SerializeObject(validatorResult, Formatting.Indented));

            //return;
            TotalsResultsAggregator defaultResultsAggregator = new TotalsResultsAggregator();
            TimeHistogramResultsAggregator histogramResultsAggregator = new TimeHistogramResultsAggregator(
                TimeSpan.FromSeconds(3),
                (result, l) => $"{TimeSpan.FromTicks(l).TotalSeconds}" // {nameAggregates[result.ThreadId%nameAggregates.Length]}
            );
            HistogramResultsAggregator customAggregator = new HistogramResultsAggregator(result => nameAggregates[result.ThreadId % nameAggregates.Length]);
            
            LoadRunnerEngine testClient =
                LoadRunnerEngine.Create<LoadTestScenario>(
                    new LoadRunnerParameters
                    {
                        Limits = new ExecutionLimits
                        {
                            MaxDuration = TimeSpan.FromSeconds(30),
                            MaxIterationsCount = Int32.MaxValue,
                            FinishTimeout = TimeSpan.FromSeconds(60)
                        },
                        ThreadingStrategy = new IncrementalWorkingThreadCount(40, 1, TimeSpan.FromSeconds(3), 3),
                        //SpeedStrategy = new IncrementalSpeed(10, TimeSpan.FromSeconds(9), 20)
                    },
                    defaultResultsAggregator, histogramResultsAggregator, customAggregator
                );
                

            testClient.Run();


            ResultsContainer results = defaultResultsAggregator.GetResults();
            List<HistogramResultRow> histogramResults = histogramResultsAggregator.GetResults().ToList();

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
            HistogramCsvExport.Export(histogramResults, "d:\\exportTest.csv");
            HistogramCsvExport.Export(customAggregator.GetResults(), "d:\\customExportTest.csv");
        }
    }

    public class LoadTestScenario : ILoadTestScenario
    {
        private static readonly Random Random = new Random(42);

        public void ScenarioSetup(ITestContext testContext)
        {
            Console.WriteLine($"ScenarioSetup {testContext.ThreadId} {testContext.ThreadIterationId} {testContext.GlobalIterationId}");
        }

        public void ScenarioTearDown(ITestContext testContext)
        {
            
            Console.WriteLine($"ScenarioTearDown {testContext.ThreadId} {testContext.ThreadIterationId} {testContext.GlobalIterationId}");
        }

        public void IterationSetup(ITestContext testContext)
        {
            Console.WriteLine($"IterationSetup {testContext.ThreadId} {testContext.ThreadId} {testContext.ThreadIterationId} {testContext.GlobalIterationId}");
            //if (Random.Next(100) % 100 == 0)
            //    throw new Exception($"#### {testContext.ThreadId} {testContext.ThreadIterationId} {testContext.GlobalIterationId}");
        }

        public void IterationTearDown(ITestContext testContext)
        {
            //if (Random.Next(100) % 50 == 0)
            //    throw new Exception($"#### {testContext.IterartionId}");
            Console.WriteLine($"IterationTearDown {testContext.ThreadId} {testContext.ThreadIterationId} {testContext.GlobalIterationId}");
        }

        public void ExecuteScenario(ITestContext testContext)
        {
            Console.WriteLine($"ExecuteScenario {testContext.ThreadId} {testContext.ThreadIterationId} {testContext.GlobalIterationId}");

            //if (Random.Next(100) % 10 == 0)
            //    throw new Exception($"@@@@ {testContext.IterartionId}");

            //Thread.Sleep(Random.Next(700));

            //testContext.Checkpoint("Checkpoint AAA");


            //if (Random.Next(100) % 50 == 0)
            //    throw new Exception("err");

            Thread.Sleep(Random.Next(50) + 50);
        }
    }
}
