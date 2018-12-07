using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Extensions;
using Viki.LoadRunner.Tools.ConsoleUi;

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

            ReplayStrategyBuilder<string> settings = new ReplayStrategyBuilder<string>()
                .SetAggregator(aggregator)
                .SetData(DataGenerator.Create(5, 1, 3, 3).ToArray())
                .SetScenario<ReplayScenario>()
                .SetThreadCount(30)
                .SetSpeed(1);

            // UI
            //LoadRunnerUi engineUi = settings.BuildUi(new DataItem(TimeSpan.Zero, "Validation demo"));
            //engineUi.StartWindow();

            // Non UI blocking
            LoadRunnerEngine engine = settings.Build();
            //engine.Run();

            // Non UI Async
            engine.RunAsync();
            engine.Wait();

            object defaultResults = aggregator.BuildResultsObjects();
            Console.WriteLine(JsonConvert.SerializeObject(defaultResults, Formatting.Indented));

            //Console.ReadKey();
        }
    }
}