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

namespace Viki.LoadRunner.Playground
{
    public class TheoreticalSpeedDemo : IScenarioFactory
    {
        List<TheoreticalScenario> _instances = new List<TheoreticalScenario>();

        public static void Run()
        {
            TheoreticalSpeedDemo scenarioFactory = new TheoreticalSpeedDemo();

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
            Console.WriteLine(watch.Elapsed.ToString("g"));


            List<TheoreticalScenario> instances = scenarioFactory._instances;
            string perThread = String.Join(Environment.NewLine, instances.Select(i => $"{i.ThreadId} {i.Count}"));
            int sum = instances.Sum(i => i.Count);
            
            Console.WriteLine(perThread);
            Console.WriteLine(@"-------");
            Console.WriteLine(sum);
            Console.WriteLine($@"TPS {sum / watch.Elapsed.TotalSeconds:N}");


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

        public IScenario Create(int threadId)
        {
            TheoreticalScenario scenario = new TheoreticalScenario();
            _instances.Add(scenario);

            return scenario;
        }
    }

    public class TheoreticalScenario : IScenario
    {
        public int Count = 0;
        public int ThreadId;

        public void ScenarioSetup(IIteration context)
        {
            ThreadId = context.ThreadId;
        }

        public void IterationSetup(IIteration context)
        {

        }

        public void ExecuteScenario(IIteration context)
        {
            Count = Count + 1;
        }

        public void IterationTearDown(IIteration context)
        {

        }

        public void ScenarioTearDown(IIteration context)
        {

        }
    }
}