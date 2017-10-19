using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay;
using Viki.LoadRunner.Engine.Strategies.Replay.Reader;

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
                .Add(new FuncMetric<int>("Threads", 0, (i, result) => result.CreatedThreads));


            ReplayStrategySettings settings = new ReplayStrategySettings
            {
                Aggregators = new IResultsAggregator[] { aggregator },
                DataReader = new ArrayDataReader(DataGenerator.Create(5, 1, 3, 3).ToArray()),
                ScenarioType = typeof(ReplayScenario),
                ThreadCount = 1
            };

            IStrategy strategy = new ReplayStrategy<string>(settings);

            LoadRunnerEngine engine = new LoadRunnerEngine(strategy);

            engine.Run();

            object defaultResults = aggregator.BuildResultsObjects();
            Console.WriteLine(JsonConvert.SerializeObject(defaultResults, Formatting.Indented));

            Console.ReadKey();
        }
    }
}