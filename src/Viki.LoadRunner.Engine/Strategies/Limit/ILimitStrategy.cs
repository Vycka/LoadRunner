using System;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Limit
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
}