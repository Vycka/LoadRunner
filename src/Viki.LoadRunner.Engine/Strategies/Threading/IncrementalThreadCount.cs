using System;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Threading
{
    public class IncrementalThreadCount : IThreadingStrategy
    {
        private readonly TimeSpan _increaseTimePeriod;

        /// <summary>
        /// Increases Created and working thread count periodically within execution of LoadTest
        /// </summary>
        /// <param name="initialThreadcount">Initial Created thread count</param>
        /// <param name="increaseTimePeriod">Delay before increasing created thread count by [increaseBatchSize]</param>
        /// <param name="increaseBatchSize">Amount of threads to create after each [increaseTimePeriod] time is reached</param>
        public IncrementalThreadCount(int initialThreadcount, TimeSpan increaseTimePeriod, int increaseBatchSize)
        {
            InitialThreadCount = initialThreadcount;
            _increaseTimePeriod = increaseTimePeriod;
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
            return (((int)(testExecutionTime.TotalMilliseconds / _increaseTimePeriod.TotalMilliseconds)) * ThreadCreateBatchSize) + InitialThreadCount;
        }
    }
}