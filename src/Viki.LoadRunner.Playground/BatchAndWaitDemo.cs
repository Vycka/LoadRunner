using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Threading;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Analytics.Metrics;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Engine.Strategies.Extensions;

namespace Viki.LoadRunner.Playground
{
    public class BatchAndWaitDemo : IScenario
    {
        private static readonly Random Random = new Random(42);

        private const int ThreadCount = 4;

        public static void Run()
        {
            HistogramAggregator histogram = new HistogramAggregator()
                .Add(new FuncDimension("ThreadIterationId", r => r.ThreadIterationId.ToString()))
                .Add(new DistinctListMetric<IResult, string>("Sleeps",r => $"{r.ThreadId}:{((int) r.UserData)}"))
                .Add(new DistinctListMetric<IResult, string>("TStarts", r => $"{r.ThreadId}:{(int)r.IterationStarted.TotalMilliseconds}"))
                .Add(new DistinctListMetric<IResult, string>("TEnds", r => $"{r.ThreadId}:{(int)r.IterationFinished.TotalMilliseconds}"))
                .Add(new FilterMetric<IResult>(r => r.ThreadId == 0, new ValueMetric<IResult>("Tr#0", r => (int)r.IterationFinished.TotalMilliseconds)))
                .Add(new FilterMetric<IResult>(r => r.ThreadId == 1, new ValueMetric<IResult>("Tr#1", r => (int)r.IterationFinished.TotalMilliseconds)))
                .Add(new FilterMetric<IResult>(r => r.ThreadId == 2, new ValueMetric<IResult>("Tr#2", r => (int)r.IterationFinished.TotalMilliseconds)))
                .Add(new FilterMetric<IResult>(r => r.ThreadId == 3, new ValueMetric<IResult>("Tr#3", r => (int)r.IterationFinished.TotalMilliseconds)))
                .Add(new FuncMetric<int>("Min TStart", Int32.MaxValue,(i, r) => Math.Min((int) r.IterationStarted.TotalMilliseconds, i)))
                .Add(new FuncMetric<int>("Max Sleep", -1, (i, r) => Math.Max(((int) r.UserData), i)))
                .Add(new FuncMetric<int>("Max TEnd", Int32.MinValue, (i, r) => Math.Max((int) r.IterationFinished.TotalMilliseconds, i)));

            StrategyBuilder strategy = new StrategyBuilder()
                .SetScenario<BatchAndWaitDemo>()
                .AddSpeed(new BatchBySlowestSpeed(ThreadCount))
                .SetThreading(new FixedThreadCount(ThreadCount))
                .SetLimit(new TimeLimit(TimeSpan.FromSeconds(20)))
                .SetAggregator(histogram);

            strategy.Build().Run();

            Console.WriteLine(JsonConvert.SerializeObject(histogram.BuildResultsObjects(), Formatting.Indented));
        }

        public void ScenarioSetup(IIteration context)
        {
        }

        public void IterationSetup(IIteration context)
        {
            //BatchResults.TryAdd(context.ThreadIterationId, new Batch[ThreadCount]);
        }

        public void ExecuteScenario(IIteration context)
        {
            int sleepMs = Random.Next(500, 3000);

            context.UserData = sleepMs; 

            Thread.Sleep(sleepMs);
        }

        public void IterationTearDown(IIteration context)
        {
        }

        public void ScenarioTearDown(IIteration context)
        {
        }
    }
}