using System;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scheduler.Interfaces
{
    public enum ScheduleAction
    {
        Execute,
        Idle
        
    }

    public interface IScheduler
    {
        bool WaitForSchedule(ref bool stop);

        void Wait(ref bool stop);

        void ThreadFinished();
        void ThreadStarted();
    }

    public interface ISchedule
    {
        ITimer Timer { get; }
        
        ScheduleAction Action { get; set; }
        TimeSpan At { get; set; }
    }

    public class Schedule : ISchedule
    {
        public Schedule(ITimer timer)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));

            Timer = timer;
        }

        public ITimer Timer { get; }
        public ScheduleAction Action { get; set; } = ScheduleAction.Execute;
        public TimeSpan At { get; set; } = TimeSpan.Zero;
    }

    public static class ScheduleExtensions
    {
        public static void Execute(this ISchedule scheduler)
        {
            scheduler.Action = ScheduleAction.Execute;

            // given how scheduler works, its impossible for this value to be bigger then current value of timer
            // so this set is not needed here

            //scheduler.At = TimeSpan.Zero; 
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

        public static void Idle(this ISchedule schedule)
        {
            schedule.Action = ScheduleAction.Idle;
        }
    }
}