using System;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Executor.Timer;
using Viki.LoadRunner.Engine.Strategies;

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public interface IThreadPoolContext
    {
        IUniqueIdFactory<int> IdFactory { get; }
        ISpeedStrategy Scheduler { get; }
        object UserData { get; }
        Type Scenario { get; }
        IResultsAggregator Aggregator { get; }

        IThreadPoolStats ThreadPool { get; }
        ITimer Timer { get; }
    }
}