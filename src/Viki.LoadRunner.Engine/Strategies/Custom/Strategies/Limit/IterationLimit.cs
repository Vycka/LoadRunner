using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit
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

        bool ILimitStrategy.StopTest(ITestState state)
        {
            return _iterationsLimit <= state.GlobalIterationId - state.ThreadPool.IdleThreadCount;
        }
    }
}