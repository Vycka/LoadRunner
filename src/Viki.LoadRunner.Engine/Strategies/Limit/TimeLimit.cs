using System;
using Viki.LoadRunner.Engine.Executor.Threads;

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

        public bool StopTest(IThreadPoolContext context)
        {
            return _timeLimit <= context.Timer.Value;
        }
    }
}