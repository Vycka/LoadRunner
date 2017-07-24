using System;
using System.Threading;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
using Viki.LoadRunner.Engine.Executor.Timer;

namespace Viki.LoadRunner.Engine.Executor.Threads.Scheduler
{
    public class SchedulerEx : IScheduler, ISchedule
    {
        private bool _cancellationToken;

        private readonly ISpeedStrategyHandler _handler;
        private readonly IThreadPoolCounter _counter;

        private readonly TimeSpan _oneSecond = TimeSpan.FromSeconds(1);

        public ITimer Timer { get; }

        public SchedulerEx(ISpeedStrategyHandler handler, IThreadPoolCounter counter, ITimer timer)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            if (counter == null)
                throw new ArgumentNullException(nameof(counter));

            _handler = handler;
            _counter = counter;
            Timer = timer;
        }

        public ScheduleAction Action { get; set; } = ScheduleAction.Execute; 
        public TimeSpan At { get; set; } = TimeSpan.Zero;

        public virtual void Idle(TimeSpan delay)
        {
            At = Timer.Value + delay;

            Action = ScheduleAction.Idle;
        }

        public virtual void ExecuteAt(TimeSpan at)
        {
            At = at;

            Action = ScheduleAction.Execute;
        }

        public TimeSpan CalculateDelay()
        {
            return At - Timer.Value;
        }

        public void Wait()
        {
            _handler.Next(this);

            TimeSpan delay = CalculateDelay();
            if (delay > TimeSpan.Zero)
            {
                _counter.AddIdle(1);

                while (Action == ScheduleAction.Idle && _cancellationToken == false)
                {
                    SemiWait(delay);

                    _handler.Next(this);
                }

                SemiWait(delay);

                _counter.AddIdle(-1);
            }
        }

        private void SemiWait(TimeSpan delay)
        {
            while (delay > _oneSecond && _cancellationToken == false)
            {
                Thread.Sleep(_oneSecond);
                delay = CalculateDelay();
            }
            if (delay > TimeSpan.Zero && _cancellationToken == false)
                Thread.Sleep(delay);
        }
    }
}