using System;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace LoadRunner.Demo.Common
{
    public class SleepingScenario : IScenario
    {
        private readonly TimeSpan _sleepTime;

        public SleepingScenario(TimeSpan sleepTime)
        {
            _sleepTime = sleepTime;
        }

        public void ScenarioSetup(IIteration context)
        {
        }

        public void IterationSetup(IIteration context)
        {
        }

        public void ExecuteScenario(IIteration context)
        {
            Thread.Sleep(_sleepTime);
        }

        public void IterationTearDown(IIteration context)
        {
        }

        public void ScenarioTearDown(IIteration context)
        {
        }
    }
}