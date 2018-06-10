using System;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Interfaces
{
    /// <summary>
    /// LoadRunner engine configuration root
    /// </summary>
    public interface ICustomStrategySettings : IAggregatorFeature, IUserDataFeature, ITimeoutFeature
    {
        /// <summary>
        /// Limits define when test execution will be scheduled to stop.
        /// Keep in mind that limits can't enforce stopping precisely how defined. E.g. you can't make it stop at exactly at 2000 iterations, but it will be close to it. 
        /// Limits precision correlates to LoadRunnerEngine.HeartBeatMs
        /// </summary>
        ILimitStrategy[] Limits { get; }

        /// <summary>
        /// Speed strategies will limit executed iteration per second.
        /// See this.SpeedPriority for prioritization.
        /// </summary>
        ISpeedStrategy[] Speeds { get; }

        /// <summary>
        /// Threading strategy controls created thread count throughout the LoadTest.
        /// </summary>
        IThreadingStrategy Threading { get; }

        /// <summary>
        /// Factory for creating IScenario instances.
        /// </summary>
        IScenarioFactory ScenarioFactory { get; }
    }
}

