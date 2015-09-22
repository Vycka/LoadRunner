using System;

namespace Viki.LoadRunner.Engine.Parameters
{
    /// <summary>
    /// Defines Test-stopping related parameters
    /// (Defaults - MaxDuration: 30sec, FinishTimeout 60sec, MaxIterationsCount Unlimited)
    /// </summary>
    public class ExecutionLimits
    {
        /// <summary>
        /// Maximum amount of time allowed for tests to run (Default 30 sec)
        /// </summary>
        public TimeSpan MaxDuration = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Maximum amount of time to wait for worker threads to finish before killing them
        /// after a MaxDuration or MaxIterationsCount or if tests are stopped due to unhandled execution errors.
        /// (Default: 60 sec)
        /// </summary>
        public TimeSpan FinishTimeout = TimeSpan.FromSeconds(60);

        /// <summary>
        /// Maximum allowed count of iterations to execute (Default: Unlimited)
        /// </summary>
        public int MaxIterationsCount = Int32.MaxValue;
    }
}