using System;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Threading
{
    public class SemiAutoThreadCount : IThreadingStrategy
    {
        private readonly int _maxThreadCount;

        /// <summary>
        /// Initially creates [minThreadCount].
        /// if within LoadTest execution there are no idle threads available to work, but oter limitations alow new iteration to be enqueued
        /// A new threads will be gradually increased, until there is no need for new threads or [maxThreadCount] limit was reached
        /// </summary>
        /// <param name="minThreadCount">initial thread count to create</param>
        /// <param name="maxThreadCount">Maximum alowed thread count to be created within LoadTest execution</param>
        public SemiAutoThreadCount(int minThreadCount, int maxThreadCount)
        {
            if (minThreadCount > maxThreadCount)
                throw new Exception("minThreadCount must lower than or equal to maxThreadCount");

            InitialThreadCount = minThreadCount;
            _maxThreadCount = maxThreadCount;
        }

        public int InitialThreadCount { get; private set; }

        public int ThreadCreateBatchSize { get; } = 1;

        public int GetAllowedMaxWorkingThreadCount(TimeSpan testExecutionTime, WorkerThreadStats workerThreadStats)
        {
            return _maxThreadCount;
        }

        public int GetAllowedCreatedThreadCount(TimeSpan testExecutionTime, WorkerThreadStats workerThreadStats)
        {
            if (
                workerThreadStats.CreatedThreadCount < _maxThreadCount &&
                workerThreadStats.InitializedThreadCount == workerThreadStats.WorkingThreadCount
            )
                return workerThreadStats.CreatedThreadCount + 1;
            else
                return workerThreadStats.CreatedThreadCount;
        }
    }
}