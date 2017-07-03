using System;
using Viki.LoadRunner.Engine.Executor.Timer;

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public enum ScheduleAction
    {
        Idle,
        Execute
    }

    public interface IScheduler
    {
        ITimer Timer { get; }

        ScheduleAction Action { get;  }
        TimeSpan At { get;  }

        void Idle(TimeSpan delay);
        void ExecuteAt(TimeSpan at);
    }

    public static class SchedulerExtensions
    {
        public static void Execute(this IScheduler scheduler)
        {
            scheduler.ExecuteAt(TimeSpan.Zero);
        }

        public static void DelayAndExecute(this IScheduler scheduler, TimeSpan delay)
        {
            TimeSpan at = scheduler.Timer.Value + delay;

            scheduler.ExecuteAt(at);
        }
    }
}