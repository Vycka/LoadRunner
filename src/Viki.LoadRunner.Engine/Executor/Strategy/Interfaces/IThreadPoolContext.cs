using System;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.Factory.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Strategy.Interfaces
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