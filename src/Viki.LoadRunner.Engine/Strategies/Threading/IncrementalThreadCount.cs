using System;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Threading
{
    public class IncrementalThreadCount : IThreadingStrategy
    {
        private readonly TimeSpan _increasePeriod;

        public IncrementalThreadCount(int initialThreadcount, TimeSpan increasePeriod, int increaseBatchSize)
        {
            InitialThreadCount = initialThreadcount;
            _increasePeriod = increasePeriod;
            ThreadCreateBatchSize = increaseBatchSize;
        }

        public int InitialThreadCount { get; }
        public int ThreadCreateBatchSize { get; }

        public int GetAllowedMaxWorkingThreadCount(TimeSpan testExecutionTime, WorkerThreadStats workerThreadStats)
        {
            return workerThreadStats.CreatedThreadCount;
        }

        public int GetAllowedCreatedThreadCount(TimeSpan testExecutionTime, WorkerThreadStats workerThreadStats)
        {
            return (((int)(testExecutionTime.TotalMilliseconds / _increasePeriod.TotalMilliseconds)) * ThreadCreateBatchSize) + InitialThreadCount;
        }
    }
}