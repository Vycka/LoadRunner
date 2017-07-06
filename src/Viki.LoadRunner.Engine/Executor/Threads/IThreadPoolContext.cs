using System;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Executor.Timer;

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public interface IThreadPoolContext
    {
        // Kinda responsibility breach, ILimitStrategy can generate Id's
        IUniqueIdFactory<int> IdFactory { get; }
        object UserData { get; }
        Type Scenario { get; }
        IResultsAggregator Aggregator { get; }

        IThreadPoolStats ThreadPool { get; }
        ITimer Timer { get; }
    }
}