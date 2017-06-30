using System;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Speed;
using Viki.LoadRunner.Engine.Strategies.Threading;

namespace Viki.LoadRunner.Engine.Parameters
{
    /// <summary>
    /// LoadRunner engine configuration root
    /// </summary>
    public interface ILoadRunnerSettings
    {
        /// <summary>
        /// Limits strategy defines Test-stopping related parameters
        /// </summary>
        ILimitStrategy Limits { get; }

        /// <summary>
        /// Speed strategy defines limitations related to executed iteration-per-second capping
        /// </summary>
        ISpeedStrategy Speed { get; }

        /// <summary>
        /// Threading strategy defines created and working parallel thread count throughout the LoadTest
        /// </summary>
        IThreadingStrategy Threading { get; }

        /// <summary>
        /// This object-value will be set to testContext.UserData for each created test thread.
        /// </summary>
        object InitialUserData { get; }
    }

    /// <summary>
    /// LoadRunner configuration root class
    /// </summary>
    public class LoadRunnerParameters : ILoadRunnerSettings
    {
        /// <summary>
        /// Limits defines Test-stopping related parameters
        /// (Defaults are unlimited except for 3 min timeout)
        /// </summary>
        public ILimitStrategy Limits { get; set; } = new LimitStrategy();
        /// <summary>
        /// SpeedStrategy defines limitations related to executed iteration-per-second capping.
        /// (Default: Unlimited, aka FixedSpeed(Double.MaxValue))
        /// </summary>
        public ISpeedStrategy Speed { get; set; } = new FixedSpeed(Double.MaxValue);
        /// <summary>
        /// ThreadingStrategy Defines Created and Working parallel thread count throughout the LoadTest
        /// (Default: 10Threads)
        /// </summary>
        public IThreadingStrategy Threading { get; set; } = new SemiAutoThreadCount(10, 10);

        /// <summary>
        /// This object-value will be set to testContext.UserData for each created test thread.
        /// </summary>
        public object InitialUserData { get; set; } = null;
    }
}