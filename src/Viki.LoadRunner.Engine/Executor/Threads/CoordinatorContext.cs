using System;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Executor.Timer;
using Viki.LoadRunner.Engine.Strategies;

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public class CoordinatorContext
    {
        public IUniqueIdFactory<int> IdFactory;
        public ISpeedStrategy Scheduler;
        public object UserData;
        public Type Scenario;
        public IResultsAggregator Aggregator;

        public IThreadPoolStats ThreadPool { get; set; }
        public ITimer Timer { get; set; }
    }
}