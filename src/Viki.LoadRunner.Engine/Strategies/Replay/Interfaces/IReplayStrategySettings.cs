using System;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Reader.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Interfaces
{
    public interface IReplayStrategySettings
    {
        int ThreadCount { get; }
        IReplayDataReader DataReader { get; }
        double SpeedMultiplier { get; }
        IResultsAggregator[] Aggregators { get; }

        Type ScenarioType { get; }
        object InitialUserData { get; }
        TimeSpan FinishTimeout { get; }
    }
}