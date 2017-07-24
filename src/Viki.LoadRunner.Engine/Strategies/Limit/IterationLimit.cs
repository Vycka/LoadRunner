using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Limit
{
    /// <summary>
    /// Stops test execution after executing defined count of iterations.
    /// </summary>
    public class IterationLimit : ILimitStrategy
    {
        private readonly int _iterationsLimit;

        /// <summary>
        /// Inits test execution stop after executing defined count of iterations.
        /// </summary>
        /// <param name="iterationsLimit">Count of iterations after which to schedule stop</param>
        public IterationLimit(int iterationsLimit)
        {
            _iterationsLimit = iterationsLimit;
        }

        bool ILimitStrategy.StopTest(IThreadPoolContext context)
        {
            return _iterationsLimit <= context.IdFactory.Current - context.ThreadPool.IdleThreadCount;
        }
    }
}