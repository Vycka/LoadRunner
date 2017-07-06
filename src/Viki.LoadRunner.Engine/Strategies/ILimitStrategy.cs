using System;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies
{
    /// <summary>
    /// Limit-Strategy controls when test execution should be stopped and how gracefully
    /// </summary>
    public interface ILimitStrategy
    {
        /// <summary>
        /// StopTest() gets called before each iteration gets enqueued.
        /// once returned value is [true] engine will initiate graceful shutdown respecting [FinishTimeout] value
        /// </summary>
        bool StopTest(IThreadPoolContext context);

        /// <summary>
        /// Time threshold how long engine should give worker-threads to finish gracefully once test is being stopped.
        /// Once threshold is reached, worker-threads will be killed with Thread.Abort() and collected iteration [IResult] value will be lost.
        /// </summary>
        TimeSpan FinishTimeout { get; }
    }

    /// <summary>
    /// Default limiting strategy
    /// </summary>
    public class LimitStrategy : ILimitStrategy
    {
        /// <summary>
        /// Maximum amount of time allowed for tests to run (Default TimeSpan.MaxValue)
        /// </summary>
        public TimeSpan MaxDuration { get; set; } = TimeSpan.MaxValue;

        /// <summary>
        /// Maximum allowed count of iterations to execute (Default: Int32.MaxValue)
        /// </summary>
        public int MaxIterationsCount { get; set; } = Int32.MaxValue;

        bool ILimitStrategy.StopTest(IThreadPoolContext context)
        {
            return context.Timer.Value >= MaxDuration || context.IdFactory.Current >= MaxIterationsCount;
        }

        /// <summary>
        /// Maximum amount of time to wait for worker threads to finish before killing them
        /// after a MaxDuration or MaxIterationsCount or if tests are stopped due to unhandled execution errors.
        /// (Default: TimeSpan.FromMinutes(3))
        /// </summary>
        public TimeSpan FinishTimeout { get; set; } = TimeSpan.FromMinutes(3);
    }
}