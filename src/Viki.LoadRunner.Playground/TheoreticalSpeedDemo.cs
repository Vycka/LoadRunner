using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Engine.Strategies.Extensions;
using Viki.LoadRunner.Playground.Tools;

namespace Viki.LoadRunner.Playground
{
    public class TheoreticalSpeedDemo
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

            /*
                i5-4670
                0:00:10,0204332
                0 20196083
                1 20441812
                2 20276545
                3 20205667
                -------
                81120107
                TPS 8 095 469,07
             */
        }


    }
}