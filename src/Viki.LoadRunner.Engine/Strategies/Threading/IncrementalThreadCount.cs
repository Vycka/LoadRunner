using System;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Executor.Result;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Threading
{
    public class IncrementalThreadCount : IThreadingStrategyLegacy, IThreadingStrategy, IDimension
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

        public void Setup(CoordinatorContext context, IThreadPoolControl control)
        {
            control.StartWorkersAsync(InitialThreadCount);
        }

        public void Adjust(CoordinatorContext context, IThreadPoolControl control)
        {
            int threadCount = (((int)(context.Timer.Value.Ticks / _increaseTimePeriod.Ticks)) * ThreadCreateBatchSize) + InitialThreadCount;

            control.SetWorkerCountAsync(threadCount);
        }

        public string DimensionName => "Created Threads";

        public string GetKey(IResult result)
        {
            return GetAllowedCreatedThreadCount(result.IterationStarted, default(WorkerThreadStats)).ToString();
        }


    }
}