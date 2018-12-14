using System;
using System.Threading;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Engine.Strategies.Extensions;
using Viki.LoadRunner.Tools.ConsoleUi;

namespace Viki.LoadRunner.Playground
{
    public class LimitConcurrencyAndTpsDemo : IScenario
    {
        public static void Run()
        {
            HistogramAggregator aggregator = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(10)))
                .Add(new FuncMetric<int>("Working Threads", 0, (i, r) => Math.Max(r.CreatedThreads - r.IdleThreads, i)))
                .Add(new CountMetric(Checkpoint.NotMeassuredCheckpoints))
                .Add(new TransactionsPerSecMetric());

            KpiPrinterAggregator kpi = new KpiPrinterAggregator(
                TimeSpan.FromSeconds(2),
                new FuncMetric<string>("T+", "???", (s, result) => result.IterationFinished.ToString("g")),
                new FuncMetric<int>("Working Threads", 0, (i, r) => Math.Max(r.CreatedThreads - r.IdleThreads, i)),
                new CountMetric(Checkpoint.NotMeassuredCheckpoints),
                new CountMetric(i => i / 2.0, Checkpoint.NotMeassuredCheckpoints) { Prefix = "TPS " }
            );

            StrategyBuilder strategy = new StrategyBuilder()
                .SetScenario<LimitConcurrencyAndTpsDemo>()
                .AddSpeed(new LimitWorkingThreads(11))
                .AddSpeed(new IncrementalSpeed(15, TimeSpan.FromSeconds(20), 15))
                .SetThreading(new FixedThreadCount(100))
                .SetLimit(new TimeLimit(TimeSpan.FromSeconds(61)))
                .SetAggregator(aggregator, kpi);

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
            //Console.WriteLine(context.Timer.Value.ToString("g"));
            Thread.Sleep(400);
        }

        public void IterationTearDown(IIteration context)
        {
        }

        public void ScenarioTearDown(IIteration context)
        {
        }
    }
}