using System;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace LoadRunner.Demo.Common
{
    public class SleepingScenarioFactory : IScenarioFactory
    {
        private readonly TimeSpan _sleepTime;

        public SleepingScenarioFactory(TimeSpan sleepTime)
        {
            _sleepTime = sleepTime;
        }

        public IScenario Create(int threadId)
        {
            return new SleepingScenario(_sleepTime);
        }
    }
}