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

        ScheduleAction Action { get; set; }
        TimeSpan At { get; set; }

        void Idle(TimeSpan delay);
        void ExecuteAt(TimeSpan at);
    }

    public interface ISchedule
    {
        ITimer Timer { get; }

        ScheduleAction Action { get; set; }
        TimeSpan At { get; set; }
    }

    public static class SchedulerExtensions
    {
        public static void Execute(this ISchedule scheduler)
        {
            scheduler.Action = ScheduleAction.Execute;
            scheduler.At = TimeSpan.Zero; // given how scheduler uses these properties, its probably impossible for this value to be bigger then current value of timer, so this set is probably not needed here
        }

        public static void DelayAndExecute(this ISchedule schedule, TimeSpan delay)
        {
            TimeSpan at = schedule.Timer.Value + delay;

            schedule.ExecuteAt(at);
        }

        public static void ExecuteAt(this ISchedule schedule, TimeSpan at)
        {
            schedule.At = at;

            schedule.Action = ScheduleAction.Execute;
        }

        public static void Idle(this ISchedule schedule, TimeSpan delay)
        {
            schedule.At = schedule.Timer.Value + delay;

            schedule.Action = ScheduleAction.Idle;
        }
    }
}