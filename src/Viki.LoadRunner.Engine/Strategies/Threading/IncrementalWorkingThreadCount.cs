using System;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Threading
{
    public class IncrementalWorkingThreadCount : IThreadingStrategy
    {
        private readonly TimeSpan _increasePeriod;
        private readonly int _increaseBatchSize;
        private readonly int _createdThreadCount;
        private readonly int _initialWorkingThreadCount;

        public IncrementalWorkingThreadCount(int createdThreadCount, int initialWorkingThreadCount, TimeSpan increasePeriod, int increaseBatchSize)
        {
            _increasePeriod = increasePeriod;
            _increaseBatchSize = increaseBatchSize;
            _createdThreadCount = createdThreadCount;
            _initialWorkingThreadCount = initialWorkingThreadCount;
        }

        public int InitialThreadCount => _createdThreadCount;

        public int ThreadCreateBatchSize { get; } = 1;

        public int GetAllowedMaxWorkingThreadCount(TimeSpan testExecutionTime, WorkerThreadStats workerThreadStats)
        {
            return (((int)(testExecutionTime.TotalMilliseconds / _increasePeriod.TotalMilliseconds)) * _increaseBatchSize) + _initialWorkingThreadCount;
        }

        public int GetAllowedCreatedThreadCount(TimeSpan testExecutionTime, WorkerThreadStats workerThreadStats)
        {
            return _createdThreadCount;
        }
    }
}