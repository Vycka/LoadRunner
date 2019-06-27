using System;
using System.Transactions;

namespace Viki.LoadRunner.Engine.Interfaces
{
    public interface IStrategyExecutor 
    {
        /// <summary>
        /// Start LoadTest execution on main thread. This blocks until test execution is finished by defined rules if any.
        /// </summary>
        void Run();

        /// <summary>
        /// Started event is triggered only if executor succeeds initialization, but moments before starting the strategy.
        /// If strategy fails at start or in the middle of execution, Stopped event will be triggered and it will contain related exception.
        /// </summary>
        event ExecutorStartedEventDelegate Started;

        /// <summary>
        /// Ended event is triggered after test is stopped.
        /// If passed exception is not not null, test has stopped abnormally and passed exception will be the reason why execution was stopped.
        /// </summary>
        event ExecutorStoppedEventDelegate Stopped;
    }

    public delegate void ExecutorStartedEventDelegate(IStrategyExecutor sender);

    public delegate void ExecutorStoppedEventDelegate(IStrategyExecutor sender, Exception exception);
}