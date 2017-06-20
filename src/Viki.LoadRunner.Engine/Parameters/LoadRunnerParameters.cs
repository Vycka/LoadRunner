using System;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Speed;
using Viki.LoadRunner.Engine.Strategies.Threading;

namespace Viki.LoadRunner.Engine.Parameters
{
    /// <summary>
    /// LoadRunner configuration root class
    /// </summary>
    public class LoadRunnerParameters
    {
        /// <summary>
        /// Limits defines Test-stopping related parameters
        /// (Defaults are unlimited except for 3 min timeout)
        /// </summary>
        public ExecutionLimits Limits = new ExecutionLimits();
        /// <summary>
        /// SpeedStrategy defines limitations related to executed iteration-per-second capping.
        /// (Default: Unlimited, aka FixedSpeed(Double.MaxValue))
        /// </summary>
        public ISpeedStrategy SpeedStrategy = new FixedSpeed(Double.MaxValue);
        /// <summary>
        /// ThreadingStrategy Defines Created and Working parallel thread count throughout the LoadTest
        /// (Default: 10Threads)
        /// </summary>
        public IThreadingStrategy ThreadingStrategy = new SemiAutoThreadCount(10, 10);

        /// <summary>
        /// This object-value will be set to testContext.UserData for each created test thread.
        /// </summary>
        public object InitialUserData = null;
    }
}