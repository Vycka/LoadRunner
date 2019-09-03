using System;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed
{
    /// <summary>
    /// Executes iterations in desired batch sizes at start of each time(frame) interval.
    /// Thread count should be at least big as batch size for strategy to work as expected
    /// </summary>
    public class BatchByTimeIntervalSpeed : ISpeedStrategy
    {
        private readonly TimeSpan _pollInterval = TimeSpan.FromMilliseconds(100);

        private readonly TimeSpan _interval;
        private readonly int _batchSize;

        protected int CurrentBatch { get; private set; }
        private int _executedBatchIterations;

        /// <summary>
        /// Executes iterations in desired batch sizes at start of each time(frame) interval.
        /// Thread count should be at least as big as batch size for strategy to work as expected.
        /// 
        /// Aim interval and thread count to be good enough so there will be enough free threads as the batch size at the start of the batch,
        /// otherwise if there are not enough threads to start whole batch at the same time
        /// there will be a late executions within batch size and time limits if any threads free up.
        /// 
        /// If no threads are available to execute the whole batch within interval,
        /// the remaining iterations won't be executed when next batch time frame comes.
        /// </summary>
        /// <param name="interval">Time interval at start of each [batchSize] of iterations will be executed</param>
        /// <param name="batchSize">Amount of iterations to execute within one batch defined in [interval]</param>
        public BatchByTimeIntervalSpeed(TimeSpan interval, int batchSize)
        {
            _interval = interval;
            _batchSize = batchSize;
        }

        public void Setup(ITestState state)
        {
            CurrentBatch = 0;
            _executedBatchIterations = 0;
        }

        public void Next(IIterationId id, ISchedule scheduler)
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

        public void ThreadStarted(IIterationId id, ISchedule scheduler)
        {
        }

        public void ThreadFinished(IIterationId id, ISchedule scheduler)
        {
        }
    }
}