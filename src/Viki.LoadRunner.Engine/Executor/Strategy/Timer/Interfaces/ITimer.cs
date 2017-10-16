using System;

namespace Viki.LoadRunner.Engine.Executor.Strategy.Timer.Interfaces
{
    /// <summary>
    /// Timer interface used to pass read-only  timer to TestContext from LoadRunner engine
    /// </summary>
    public interface ITimer
    {
        /// <summary>
        /// Time passed since the start of the execution
        /// </summary>
        TimeSpan Value { get; }

        /// <summary>
        /// Indicates whether timer is running
        /// </summary>
        bool IsRunning { get;  }
        
        /// <summary>
        /// Real-world utc time of when load test was started
        /// </summary>
        DateTime BeginTime { get; }
    }
}