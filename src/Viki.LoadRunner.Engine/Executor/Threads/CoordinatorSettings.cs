using System;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Executor.Timer;
using Viki.LoadRunner.Engine.Strategies;

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public class CoordinatorSettings
    {
        public Type Scenario;
        public ITimer Timer;
        public ISpeedStrategy Scheduler;
        public IResultsAggregator Aggregator;
        public object InitialUserData;
    }
}