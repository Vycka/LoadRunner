using System;
using Viki.LoadRunner.Engine.Core.Pool.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed
{
    public class LimitWorkingThreads : ISpeedStrategy
    {
        /// <summary>
        /// Count of worker-threads to allow executing iterations
        /// </summary>
        public int WorkingThreads;

        /// <summary>
        /// How much time to wait before thread will try to enqueue for iteration again
        /// </summary>
        public TimeSpan DelayInterval = TimeSpan.FromMilliseconds(100);

        private IThreadPoolStats _pool;

        public LimitWorkingThreads(int workingThreads)
        {
            WorkingThreads = workingThreads;
        }

        public void Setup(ITestState state)
        {
            _pool = state.ThreadPool;
        }

        public void Next(IIterationId id, ISchedule scheduler)
        {
            // TODO: Bug lets say all threads try to check at the same time and all will see no working threads and all will start.

            int includeSelf = scheduler.Action == ScheduleAction.Execute ? 1 : 0; 
            int workingThreads = _pool.CreatedThreadCount - _pool.IdleThreadCount - includeSelf;
            if (workingThreads < WorkingThreads)
            {
                scheduler.Execute();
            }
            else
            {
                scheduler.Idle(DelayInterval);
            }
        }

        public void HeartBeat(ITestState state)
        {
        }
    }
}