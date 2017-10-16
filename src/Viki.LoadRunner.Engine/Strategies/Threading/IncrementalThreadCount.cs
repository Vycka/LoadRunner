using System;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.Pool.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.State.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.Stats.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Threading
{
    public class IncrementalThreadCount : IThreadingStrategy, IDimension
    {
        private readonly TimeSpan _increaseTimePeriod;
        private readonly int _initialThreadCount;
        private readonly int _threadCreateBatchSize;

        /// <summary>
        /// Increases Created and working thread count periodically within execution of LoadTest
        /// </summary>
        /// <param name="initialThreadcount">Initial Created thread count</param>
        /// <param name="increaseTimePeriod">Delay before increasing created thread count by [increaseBatchSize]</param>
        /// <param name="increaseBatchSize">Amount of threads to create after each [increaseTimePeriod] time is reached</param>
        public IncrementalThreadCount(int initialThreadcount, TimeSpan increaseTimePeriod, int increaseBatchSize) 
        {
            _initialThreadCount = initialThreadcount;
            _increaseTimePeriod = increaseTimePeriod;
            _threadCreateBatchSize = increaseBatchSize;
        }

        private int GetAllowedCreatedThreadCount(TimeSpan testExecutionTime)
        {
            return (((int)(testExecutionTime.TotalMilliseconds / _increaseTimePeriod.TotalMilliseconds)) * _threadCreateBatchSize) + _initialThreadCount;
        }

        public void Setup(IThreadPool pool)
        {
            pool.StartWorkersAsync(_initialThreadCount);
        }

        public void HeartBeat(IThreadPool pool, ITestState state)
        {
            int threadCount = GetAllowedCreatedThreadCount(state.Timer.Value);

            pool.SetWorkerCountAsync(state, threadCount);
        }

        public string DimensionName => "Created Threads";

        public string GetKey(IResult result)
        {
            return GetAllowedCreatedThreadCount(result.IterationStarted).ToString();
        }


    }
}