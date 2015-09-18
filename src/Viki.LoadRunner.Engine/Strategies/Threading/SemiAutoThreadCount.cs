using System;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Threading
{
    public class SemiAutoThreadCount : IThreadingStrategy
    {
        private readonly int _maxThreadCount;

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