using System;
using System.Diagnostics;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Engine.Strategies.Extensions;

namespace LoadRunner.Demo.Theoretical
{

    // No aggregation is used here, worker threads them selves will count how much iterations they have done and all will be just summed up.
    public static class TheoreticalSpeedDemo
    {
        public static void Run()
        {
            CountingScenarioFactory scenarioFactory = new CountingScenarioFactory();

            StrategyBuilder strategy = new StrategyBuilder()
                .SetScenario(scenarioFactory)
                .SetThreading(new FixedThreadCount(4))
                .SetLimit(new TimeLimit(TimeSpan.FromSeconds(10)));
            
            // Increase TimeLimit precision
            LoadRunnerEngine engine = strategy.Build();
            engine.HeartBeatMs = 1;

            Stopwatch watch = new Stopwatch();
            watch.Start();
            engine.Run();
            watch.Stop();

            scenarioFactory.PrintSum();
            Console.WriteLine($@"TPS {scenarioFactory.GetSum() / watch.Elapsed.TotalSeconds:N}");
            Console.WriteLine(watch.Elapsed.ToString("g"));
        }
    }
}