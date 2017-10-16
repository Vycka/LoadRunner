using System;
using Viki.LoadRunner.Engine.Executor.State.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Limit
{
    /// <summary>
    /// Inits test execution stop once execution time reaches defined limit.
    /// </summary>
    public class TimeLimit : ILimitStrategy
    {
        private readonly TimeSpan _timeLimit;

        /// <summary>
        /// Inits test execution stop once execution time reaches defined limit.
        /// </summary>
        /// <param name="timeLimit">How long tests should be executed</param>
        public TimeLimit(TimeSpan timeLimit)
        {
            _timeLimit = timeLimit;
        }

        public bool StopTest(ITestState state)
        {
            return _timeLimit <= state.Timer.Value;
        }
    }
}