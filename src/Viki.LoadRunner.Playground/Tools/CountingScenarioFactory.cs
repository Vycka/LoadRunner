using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Playground.Tools
{
    public class CountingScenarioFactory : IScenarioFactory
    {
        List<CountingScenario> _instances = new List<CountingScenario>();

        public IScenario Create(int threadId)
        {
            CountingScenario scenario = new CountingScenario();
            _instances.Add(scenario);

            return scenario;
        }

        public void PrintSum()
        {
            string perThread = String.Join(Environment.NewLine, _instances.Select(i => $"{i.ThreadId} {i.Count}"));
            long sum = GetSum();

            Console.WriteLine(perThread);
            Console.WriteLine(@"-------");
            Console.WriteLine(sum);
        }


        public long GetSum()
        {
            return _instances.Sum(i => i.Count);
        }

    }
}