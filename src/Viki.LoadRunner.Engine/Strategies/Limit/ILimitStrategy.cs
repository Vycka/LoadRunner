using Viki.LoadRunner.Engine.Executor.Strategy.State.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Limit
{
    /// <summary>
    /// Limit-Strategy controls when test execution should be stopped and how gracefully
    /// </summary>
    public interface ILimitStrategy
    {
        /// <summary>
        /// StopTest() gets called before each iteration gets enqueued.
        /// once returned value is [true] engine will initiate graceful shutdown respecting [FinishTimeout] value.
        /// </summary>
        bool StopTest(ITestState state);
    }
}