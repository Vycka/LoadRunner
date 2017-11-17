using System;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Reader.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Interfaces
{
    // TODO: Need to make this generic in the end... :(
    // Rethink Handler/Scheduler/IDataReader to make use of TData from IReplayStrategySettings but it should not spread like it does now.
    public interface IReplayStrategySettings<TData>
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
        /// Aggregators to collect the data
        /// </summary>
        IAggregator[] Aggregators { get; }

        /// <summary>
        /// Factory for creating IReplayScenario instances.
        /// </summary>
        IScenarioFactory ScenarioFactory { get; }

        /// <summary>
        /// Initial user data which will be passed to created thread contexts. (context.UserData)
        /// </summary>
        object InitialUserData { get; }

        /// <summary>
        /// Timeout for strategy threads to stop and cleanup.
        /// This does not affect result IAggregator and execution will still hold indefinetely until its finished.
        /// </summary>
        TimeSpan FinishTimeout { get; }
    }
}