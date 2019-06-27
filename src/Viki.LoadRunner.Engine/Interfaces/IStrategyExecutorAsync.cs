using System;

namespace Viki.LoadRunner.Engine.Interfaces
{
    public interface IStrategyExecutorAsync : IStrategyExecutor
    {
        /// <summary>
        /// If execution failed due to unhandled exception, it will be set here.
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// Is test running
        /// </summary>
        bool Running { get; }

        /// <summary>
        /// Executes test in separate thread (non-blocking call)
        /// </summary>
        void RunAsync();

        /// <summary>
        /// Cancels Async test execution.
        /// Stops exeucion gracefully with time-out handling.
        /// Aggregated data up to this point won't be lost. 
        /// </summary>
        /// <remarks>Use [Running] property to check when it is finished</remarks>
        void CancelAsync(bool blocking = false);

        /// <summary>
        /// Waits, till execution is finished gracefully with graceful waiting for ExecutionStrategy and FinishTimeout.
        /// </summary>
        /// <param name="timeOut">timeout time period to wait before returning</param>
        /// <param name="abortOnTimeOut">if execution won't finish within desired timeout, should it be terminated prematurely?</param>
        /// <returns>true - if test execution is stopped (either before timeout or aborted due to [abortOnTimeOut])</returns>
        bool Wait(TimeSpan timeOut, bool abortOnTimeOut = false);

        /// <summary>
        /// Waits Infinitely until loadtest execution is finished
        /// </summary>
        void Wait();
    }
}