using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Factory;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Replay.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Reader;
using Viki.LoadRunner.Tools.Extensions;
using Viki.LoadRunner.Tools.Windows;

namespace Viki.LoadRunner.Playground.Replay
{
    public class ReplayDemo
    {
        public static void Run()
        {
            HistogramAggregator aggregator = new HistogramAggregator()
                .Add(new FuncDimension("Iteration", result => result.GlobalIterationId.ToString()))
                .Add(new FuncDimension("Time", result => result.IterationStarted.TotalSeconds.ToString(CultureInfo.InvariantCulture)))
                .Add(new FuncDimension("Data", result => result.UserData.ToString()))
                .Add(new FuncMetric<int>("CThreads", 0, (i, r) => r.CreatedThreads))
                .Add(new FuncMetric<int>("IThreads", 0, (i, r) => r.IdleThreads));


            ReplayStrategyBuilder<string> settings = new ReplayStrategyBuilder<string>
            {
                Aggregators = new IAggregator[] { aggregator },
                DataReader = new ArrayDataReader(DataGenerator.Create(5, 1, 3, 3).ToArray()),
                ScenarioFactory = new ScenarioFactory<IReplayScenario<string>>(typeof(ReplayScenario)),
                ThreadCount = 50,
                SpeedMultiplier = 2
            };

            // UI
            LoadRunnerUi engineUi = settings.BuildUi();
            engineUi.StartWindow();

            // Non UI blocking
            //LoadRunnerEngine engine = settings.Build();
            //engine.Run();

            // Non UI Async
            //engine.RunAsync();
            //engine.Wait();

            object defaultResults = aggregator.BuildResultsObjects();
            Console.WriteLine(JsonConvert.SerializeObject(defaultResults, Formatting.Indented));

            Console.ReadKey();
        }
    }
}