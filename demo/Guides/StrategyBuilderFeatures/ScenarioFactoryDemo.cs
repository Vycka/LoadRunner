using System;
using System.Threading;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Engine.Strategies.Extensions;

namespace LoadRunner.Demo.Guides.StrategyBuilderFeatures
{
    public class ScenarioFactoryDemo
    {
        // In StrategyBuilder one can provide custom scenario factory instead of scenario type it self.
        // This can be useful if one needs a constructor in scenario or one want to use more than one type of IScenario within one test.
        public class Factory : IScenarioFactory
        {
            public IScenario Create(int threadId)
            {
                return new ScenarioWithConstructor(threadId);
            }
        }

        public class ScenarioWithConstructor : IScenario
        {
            public ScenarioWithConstructor(int id)
            {
                Console.WriteLine($"Created worker with with id {id}");
            }

            public void ScenarioSetup(IIteration context)
            {
            }

            public void IterationSetup(IIteration context)
            {
            }

            public void ExecuteScenario(IIteration context)
            {
                Thread.Sleep(10);
            }

            public void IterationTearDown(IIteration context)
            {
            }

            public void ScenarioTearDown(IIteration context)
            {
            }
        }

        public static void Run()
        {

            StrategyBuilder strategy = new StrategyBuilder()
                .SetScenario(new Factory()) // Use own custom scenario factory
                .SetLimit(new TimeLimit(TimeSpan.FromSeconds(5)))
                .SetThreading(new FixedThreadCount(4));


            LoadRunnerEngine engine = strategy.Build();
            engine.Run();

            Console.ReadKey();
        }
    }
}