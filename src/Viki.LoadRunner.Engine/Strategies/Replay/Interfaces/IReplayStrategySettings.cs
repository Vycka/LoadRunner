using System;
using System.Linq;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Reader.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Interfaces
{
    public interface IReplayStrategySettings<TData>
    {
        int ThreadCount { get; }
        IReplayDataReader DataReader { get; }
        double SpeedMultiplier { get; }
        IResultsAggregator[] Aggregators { get; }

        Type ScenarioType { get; }
        object InitialUserData { get; }
        TimeSpan FinishTimeout { get; }
    }

    public static class ReplayStrategySettingsExtensions
    {
        public static ReplayStrategyBuilder<TData> Clone<TData>(this IReplayStrategySettings<TData> settings)
        {
            return new ReplayStrategyBuilder<TData>()
            {
                ThreadCount = settings.ThreadCount,
                DataReader = settings.DataReader,
                SpeedMultiplier = settings.SpeedMultiplier,
                Aggregators = settings.Aggregators.ToArray(),

                ScenarioType = settings.ScenarioType,

                FinishTimeout = settings.FinishTimeout,
                InitialUserData = settings.InitialUserData,
            };
        }
    }
}