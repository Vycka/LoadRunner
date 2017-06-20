using System;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Threading
{
    public class FixedThreadCount : IThreadingStrategy
    {
        private readonly int _workingThreadCount;

        public FixedThreadCount(int createdThreadCount, int workingThreadCount)
        {
            _workingThreadCount = workingThreadCount;
            InitialThreadCount = createdThreadCount;
            ThreadCreateBatchSize = createdThreadCount;
        }

        public int InitialThreadCount { get; }
        public int ThreadCreateBatchSize { get; }

        public int GetAllowedMaxWorkingThreadCount(TimeSpan testExecutionTime, WorkerThreadStats workerThreadStats)
        {
            return _workingThreadCount;
        }

        public int GetAllowedCreatedThreadCount(TimeSpan testExecutionTime, WorkerThreadStats workerThreadStats)
        {
            return ThreadCreateBatchSize;
        }
    }
}