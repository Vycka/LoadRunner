using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Pool.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Adapter.Speed;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed
{
    public class LimitWorkingThreads : ISpeedStrategy
    {
        /// <summary>
        /// Count of worker-threads to allow executing iterations
        /// </summary>
        public int ThreadLimit;

        /// <summary>
        /// Tries to spread iterations between all created threads
        /// Otherwise the first threads that get the job, continue doing it until it's forced to stop
        /// </summary>
        public bool ForceSpread;

        /// <summary>
        /// How much time to wait before thread will try to enqueue for iteration again
        /// </summary>
        public TimeSpan DelayInterval = TimeSpan.FromMilliseconds(50);

        private ConditionalWeakTable<ISchedule, LastState> _states;
        private int _workingThreads;
        private IThreadPoolStats _pool;


        public LimitWorkingThreads(int workingThreads, bool forceSpread = false)
        {
            ThreadLimit = workingThreads;
            ForceSpread = forceSpread;
        }

        public void Setup(ITestState state)
        {
            _states = new ConditionalWeakTable<ISchedule, LastState>();
            _pool = state.ThreadPool;
            _workingThreads = 0;
        }

        public void Next(IIterationId id, ISchedule schedule)
        {
            LastState state = GetState(schedule);

            if (TryExecute(state))
            {
                schedule.Execute();
            }
            else
            {
                schedule.Idle(DelayInterval);
            }
        }

        private bool TryExecute(LastState state)
        {
            bool wasWorking = state.State == ScheduleAction.Execute;
            bool result = false;

            if (_workingThreads < ThreadLimit)
            {
                if (wasWorking)
                {
                    result = true;
                }
                else
                {
                    int newWorkingThreads = Interlocked.Increment(ref _workingThreads);
                    if (newWorkingThreads <= ThreadLimit)
                    {
                        state.State = ScheduleAction.Execute;
                        result = true;
                    }
                    else
                    {
                        Interlocked.Decrement(ref _workingThreads);
                        state.State = ScheduleAction.Idle;
                        //result = false;
                    }
                }

            }
            else if (wasWorking)
            {
                if (!ForceSpread || _pool.IdleThreadCount == 0)
                {
                    result = true;
                }
                else
                {
                    state.State = ScheduleAction.Idle;
                    Interlocked.Decrement(ref _workingThreads);
                    //result = false;
                }
            }

            return result;
        }

        private LastState GetState(ISchedule key)
        {
            LastState result;
            if (_states.TryGetValue(key, out result) == false)
            {
                result = new LastState();
                _states.Add(key, result);
            }
            return result;
        }

        public void HeartBeat(ITestState state)
        {
        }

        private class LastState
        {
            public ScheduleAction State = ScheduleAction.Idle;
        }
    }
}