using System;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed
{
    /// <summary>
    /// Executes iterations in desired batch sizes at each passed time interval.
    /// Thread count should be at least big as batch size for strategy to work as expected
    /// </summary>
    public class BatchingSpeed : ISpeedStrategy
    {
        private readonly TimeSpan _pollInterval = TimeSpan.FromMilliseconds(100);

        private readonly TimeSpan _interval;
        private readonly int _batchSize;

        protected int CurrentBatch { get; private set; }
        private int _executedBatchIterations;

        /// <summary>
        /// Executes iterations in desired batch sizes at each interval.
        /// 
        /// Thread count should be at least as big as batch size for strategy to work as expected.
        /// 
        /// Threads should be able to execute their iteration within the interval,
        /// otherwise they wont be executed at the same time as the rest of the batch.
        /// 
        /// If no threads are available to execute the whole batch within interval,
        /// the remaining iterations won't be executed when next time interval comes.
        /// </summary>
        /// <param name="interval">Time interval at start of each [batchSize] of iterations will be executed</param>
        /// <param name="batchSize">Amount of iterations to execute within one batch defined in [interval]</param>
        public BatchingSpeed(TimeSpan interval, int batchSize)
        {
            _interval = interval;
            _batchSize = batchSize;
        }

        public void Setup(ITestState state)
        {
            CurrentBatch = 0;
            _executedBatchIterations = 0;
        }

        public void Next(IIterationState state, ISchedule scheduler)
        {

            if (_executedBatchIterations < _batchSize)
            {
                Interlocked.Add(ref _executedBatchIterations, 1);
                scheduler.Execute();
            }
            else
            {
                scheduler.Idle(_pollInterval);
            }
        }

        public void HeartBeat(ITestState state)
        {
            int batch = (int)(state.Timer.Value.Ticks / _interval.Ticks);

            if (batch != CurrentBatch)
            {
                CurrentBatch = batch;
                Interlocked.Exchange(ref _executedBatchIterations, 0);
            }
        }
    }
}