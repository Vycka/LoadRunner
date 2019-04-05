using System;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Pool.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed
{

    /// <summary>
    /// Executes iterations at same time with desired batch size,
    /// Waits for batch to complete, and only then executes next batches in the same manner
    ///
    /// Scenario must contain at least same amount of threads as batchSize otherwise this strategy won't allow execution
    /// </summary>
    public class BatchBySlowestSpeed : ISpeedStrategy
    {
        private enum BatchState
        {
            Waiting,
            Executing
        }

        /// <summary>
        ///  How much threads should sleep before polling for new job (It must be below ExecuteDelay to prevent misalignments)
        /// </summary>
        public TimeSpan WaitDelay = TimeSpan.FromMilliseconds(10);
        /// <summary>
        /// Once all threads are available for batch, how long offset the execution, so other threads with WaitDelay in action will have enough time to receive target time.
        /// </summary>
        public TimeSpan ExecuteDelay = TimeSpan.FromMilliseconds(100);

        private readonly int _batchSize;

        private BatchState _batchState;
        private int _executedInBatch;
        private TimeSpan _executeAt;
        
        /// <summary>
        /// Executes iterations at same time with desired batch size,
        /// Waits for batch to complete, and only then executes next batches in the same manner
        ///
        /// Scenario must contain at least same amount of threads as batchSize otherwise this strategy won't allow execution
        /// </summary>
        public BatchBySlowestSpeed(int batchSize)
        {
            _batchSize = batchSize;
        }

        public void Setup(ITestState state)
        {
            _batchState = BatchState.Waiting;
        }

        public void Next(IIterationId id, ISchedule scheduler)
        {
            if (_batchState == BatchState.Executing)
            {
                int next = Interlocked.Increment(ref _executedInBatch);
                if (next <= _batchSize)
                    scheduler.ExecuteAt(_executeAt);
                else
                    scheduler.Idle(WaitDelay);
            }
            else
            {
                scheduler.Idle(WaitDelay);
            }
        }

        public void HeartBeat(ITestState state)
        {
            if (_batchState == BatchState.Waiting)
            {
                IThreadPoolStats pool = state.ThreadPool;

                if (pool.CreatedThreadCount == pool.IdleThreadCount && pool.CreatedThreadCount >= _batchSize)
                {
                    _executedInBatch = 0;
                    _executeAt = state.Timer.Value.Add(ExecuteDelay);
                    _batchState = BatchState.Executing;
                }     
            }
            else if (_executedInBatch >= _batchSize)
            {
                _batchState = BatchState.Waiting;
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