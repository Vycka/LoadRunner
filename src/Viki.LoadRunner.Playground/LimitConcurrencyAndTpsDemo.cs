using System;
using System.Threading;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Engine.Strategies.Extensions;

namespace Viki.LoadRunner.Playground
{
    public class LimitConcurrencyAndTpsDemo : IScenario
    {
        public static void Run()
        {
            HistogramAggregator aggregator = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(5)))
                .Add(new FuncMetric<int>("Working Threads", 0, (i, r) => Math.Max(r.CreatedThreads - r.IdleThreads, i)))
                .Add(new TransactionsPerSecMetric());

            StrategyBuilder strategy = new StrategyBuilder()
                .SetScenario<LimitConcurrencyAndTpsDemo>()
                .AddSpeed(new IncrementalSpeed(20, TimeSpan.FromSeconds(10), 20))
                .AddSpeed(new LimitWorkingThreads(11))
                .SetThreading(new FixedThreadCount(100))
                .SetLimit(new TimeLimit(TimeSpan.FromSeconds(16)))
                .SetAggregator(aggregator);

            strategy.Build().Run();

            Console.WriteLine(JsonConvert.SerializeObject(aggregator.BuildResultsObjects(), Formatting.Indented));
        }

        public void ScenarioSetup(IIteration context)
        {
        }

        public void IterationSetup(IIteration context)
        {
        }

        public void ExecuteScenario(IIteration context)
        {
            //Console.WriteLine(string.Concat(context.ThreadId));
            Thread.Sleep(500);
        }

        public void IterationTearDown(IIteration context)
        {
        }

        public void ScenarioTearDown(IIteration context)
        {
        }
    }
}