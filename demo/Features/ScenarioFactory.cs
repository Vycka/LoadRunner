using System;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;

namespace LoadRunner.Demo.Features
{
    public class ScenarioFactory
    {
        // In StrategyBuilder one can provide custom scenario factory instead of scenario type it self.
        // This can be useful if one needs a constructor in scenario if you want to use more than one type of IScenario within one test.
        public class Factory : IScenarioFactory
        {
            private readonly Random _rnd = new Random(42);

            public IScenario Create()
            {
                return new ScenarioWithConstructor(_rnd.Next());
            }
        }

        public class ScenarioWithConstructor : IScenario
        {
            public ScenarioWithConstructor(int something)
            {
                Console.WriteLine($"Created with {something}");
            }

            public void ScenarioSetup(IIteration context)
            {
            }

            public void IterationSetup(IIteration context)
            {
            }

            public void ExecuteScenario(IIteration context)
            {
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
                .SetSpeed(new FixedSpeed(100))
                .SetThreading(new FixedThreadCount(4));


            LoadRunnerEngine engine = strategy.Build();

            engine.Run();

            Console.ReadKey();
        }
    }
}