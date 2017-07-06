using System;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Limit;

namespace Viki.LoadRunner.Engine.Settings
{
    /// <summary>
    /// LoadRunner engine configuration root
    /// </summary>
    public interface ILoadRunnerSettings
    {
        /// <summary>
        /// Limits define when test execution will be scheduled to stop.
        /// Keep in mind that limits can't enforce stopping precisely how defined. E.g. you can't make it stop at exactly at 2000 iterations, but it will be close to it. 
        /// Limits precision strongly correlate to LoadRunnerEngine.HeartBeatMs
        /// </summary>
        ILimitStrategy[] Limits { get; }

        /// <summary>
        /// Speed strategies will limit executed iteration per second.
        /// See this.SpeedPriority for prioritization.
        /// </summary>
        ISpeedStrategy[] Speed { get; }

        /// <summary>
        /// Threading strategy defines created and working parallel thread count throughout the LoadTest
        /// </summary>
        IThreadingStrategy Threading { get; }

        /// <summary>
        /// Time threshold how long engine should give worker-threads to finish gracefully once they are scheduled to stop.
        /// If threshold is reached, worker-threads will be killed with Thread.Abort() and collected iteration [IResult] value will be lost.
        /// </summary>
        TimeSpan FinishTimeout { get; }

        /// <summary>
        /// Scenario to execute, type must implement ILoadTestScenario.
        /// </summary>
        Type TestScenarioType { get; }

        /// <summary>
        /// This object-value will be set to testContext.UserData for each created test thread.
        /// </summary>
        object InitialUserData { get; }
    }
}

