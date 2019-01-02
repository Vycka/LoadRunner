using System;
using Viki.LoadRunner.Engine.Core.Counter.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Data;
using Viki.LoadRunner.Engine.Strategies.Replay.Data.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Scheduler
{
    public class ReplayScheduler : IScheduler
    {
        private readonly ITimer _timer;
        private readonly IReplayScenarioHandler _scenarioHandler;
        private readonly IReplayDataReader _dataReader;
        private readonly IThreadPoolCounter _counter;
        private readonly double _speedMultiplier;
        private readonly int _threadId;

        private readonly IWait _waiter;

        public ReplayScheduler(ITimer timer, IReplayScenarioHandler scenarioHandler, IReplayDataReader dataReader, IThreadPoolCounter counter, double speedMultiplier, int threadId)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));
            if (scenarioHandler == null)
                throw new ArgumentNullException(nameof(scenarioHandler));
            if (dataReader == null)
                throw new ArgumentNullException(nameof(dataReader));
            if (counter == null)
                throw new ArgumentNullException(nameof(counter));
            if (speedMultiplier <= 0)
                throw new ArgumentException("speedMultiplier must be above 0", nameof(speedMultiplier));

            _timer = timer;
            _scenarioHandler = scenarioHandler;
            _dataReader = dataReader;
            _counter = counter;
            _speedMultiplier = speedMultiplier;
            _threadId = threadId;

            _waiter = new SemiWait(timer);
        }

        public void WaitNext(ref bool stop)
        {
            DataItem dataItem = _dataReader.Next(_threadId);

            if (dataItem != null)
            {
                TimeSpan adjustedTarget = TimeSpan.FromTicks((long)(dataItem.TimeStamp.Ticks / _speedMultiplier));

                bool execute = _scenarioHandler.SetData(dataItem.Value, adjustedTarget);

                if (execute)
                {
                    if (adjustedTarget > _timer.Value)
                    {
                        _counter.AddIdle(1);

                        _waiter.Wait(adjustedTarget, ref stop);

                        _counter.AddIdle(-1);
                    }
                }
            }
            else
            {
                stop = true;
            }
        }

        public void ThreadStarted()
        {
        }
        public void ThreadFinished()
        {
        }
    }
}