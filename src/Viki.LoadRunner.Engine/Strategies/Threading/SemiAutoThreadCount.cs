using System;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Threading
{
    /// <summary>
    /// Create Fixed amount of working threads or try SemiAutoStrategy dictate its count
    /// </summary>
    public class SemiAutoThreadCount : IThreadingStrategy
    {
        private readonly int _maxThreadCount;
        private readonly int _initialThreadCount;
        /// <summary>
        /// Initially creates [minThreadCount].
        /// if within LoadTest execution there are no idle threads available to work, but oter limitations alow new iteration to be enqueued
        /// A new threads will be gradually created, until there is no need for new threads or [maxThreadCount] limit was reached
        /// </summary>
        /// <param name="minThreadCount">initial thread count to create</param>
        /// <param name="maxThreadCount">Maximum alowed thread count to be created within LoadTest execution</param>
        public SemiAutoThreadCount(int minThreadCount, int maxThreadCount)
        {
            if (minThreadCount > maxThreadCount)
                throw new Exception("minThreadCount must lower than or equal to maxThreadCount");

            _initialThreadCount = minThreadCount;
            _maxThreadCount = maxThreadCount;
        }

        int IThreadingStrategy.InitialThreadCount => _initialThreadCount;

        int IThreadingStrategy.ThreadCreateBatchSize { get; } = 1;

        int IThreadingStrategy.GetAllowedMaxWorkingThreadCount(TimeSpan testExecutionTime, WorkerThreadStats workerThreadStats)
        {
            return _maxThreadCount;
        }

        int IThreadingStrategy.GetAllowedCreatedThreadCount(TimeSpan testExecutionTime, WorkerThreadStats workerThreadStats)
        {
            if (
                workerThreadStats.CreatedThreadCount < _maxThreadCount &&
                workerThreadStats.CreatedThreadCount == workerThreadStats.WorkingThreadCount
            )
                return workerThreadStats.CreatedThreadCount + 1;
            else
                return workerThreadStats.CreatedThreadCount;
        }
    }
}