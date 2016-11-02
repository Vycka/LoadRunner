using System;

namespace Viki.LoadRunner.Engine.Parameters
{
    /// <summary>
    /// Defines Test-stopping related parameters
    /// (Defaults are unlimited except for 60 sec timeout)
    /// </summary>
    public class ExecutionLimits
    {
        /// <summary>
        /// Maximum amount of time allowed for tests to run (Default TimeSpan.MaxValue)
        /// </summary>
        public TimeSpan MaxDuration = TimeSpan.MaxValue;

        /// <summary>
        /// Maximum amount of time to wait for worker threads to finish before killing them
        /// after a MaxDuration or MaxIterationsCount or if tests are stopped due to unhandled execution errors.
        /// (Default: TimeSpan.FromSeconds(60))
        /// </summary>
        public TimeSpan FinishTimeout = TimeSpan.FromSeconds(60);

        /// <summary>
        /// Maximum allowed count of iterations to execute (Default: Int32.MaxValue)
        /// </summary>
        public int MaxIterationsCount = Int32.MaxValue;
    }
}