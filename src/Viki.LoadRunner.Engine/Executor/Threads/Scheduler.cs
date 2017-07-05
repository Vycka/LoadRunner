using System;
using Viki.LoadRunner.Engine.Executor.Timer;

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public class Scheduler : IScheduler
    {
        public ITimer Timer { get; }

        public Scheduler(ITimer timer)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));

            Timer = timer;
        }

        public ScheduleAction Action { get; protected set; }
        public TimeSpan At { get; protected set; }

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

        public TimeSpan CalculateDelta()
        {
            return At - Timer.Value;
        }
    }
}