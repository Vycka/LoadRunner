using System;
using Viki.LoadRunner.Engine.Strategies.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Data.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Factory.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Interfaces
{
    public interface IReplayStrategySettings<in TData> : IAggregatorFeature, IUserDataFeature, ITimeoutFeature
    {
        /// <summary>
        /// Fixed count of threads to use
        /// </summary>
        int ThreadCount { get; }

        /// <summary>
        /// Datasource which defines timeline of replay execution
        /// </summary>
        IReplayDataReader DataReader { get; }

        /// <summary>
        /// Speed multiplier at which Replay strategy will run
        /// </summary>
        double SpeedMultiplier { get; }

        /// <summary>
        /// Factory for creating IReplayScenario instances.
        /// </summary>
        IReplayScenarioFactory<TData> ScenarioFactory { get; }
    }
}