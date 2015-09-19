using System;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Threading
{
    public class IncrementalWorkingThreadCount : IThreadingStrategy
    {
        private readonly TimeSpan _increaseTimePeriod;
        private readonly int _increaseBatchSize;
        private readonly int _createdThreadCount;
        private readonly int _initialWorkingThreadCount;

        /// <summary>
        /// Creates fixed [createdThreadCount] amount of threads, and always balances iterations within all of them,
        /// but limites concurrent-working-threads by provided rules
        /// </summary>
        /// <param name="createdThreadCount">Fixed amount of threads to create</param>
        /// <param name="initialWorkingThreadCount">Initial amount of concurrent-working-threads</param>
        /// <param name="increaseTimePeriod">After what duration concurrent-working-thread limit will be increased by [increaseBatchSize]</param>
        /// <param name="increaseBatchSize">How much concurrent-working-thread count will be increased each time the [increaseTimePeriod] hits</param>
        public IncrementalWorkingThreadCount(int createdThreadCount, int initialWorkingThreadCount, TimeSpan increaseTimePeriod, int increaseBatchSize)
        {
            _increaseTimePeriod = increaseTimePeriod;
            _increaseBatchSize = increaseBatchSize;
            _createdThreadCount = createdThreadCount;
            _initialWorkingThreadCount = initialWorkingThreadCount;
        }

        public int InitialThreadCount => _createdThreadCount;

        public int ThreadCreateBatchSize { get; } = 1;

        public int GetAllowedMaxWorkingThreadCount(TimeSpan testExecutionTime, WorkerThreadStats workerThreadStats)
        {
            return (((int)(testExecutionTime.TotalMilliseconds / _increaseTimePeriod.TotalMilliseconds)) * _increaseBatchSize) + _initialWorkingThreadCount;
        }

        public int GetAllowedCreatedThreadCount(TimeSpan testExecutionTime, WorkerThreadStats workerThreadStats)
        {
            return _createdThreadCount;
        }
    }
}