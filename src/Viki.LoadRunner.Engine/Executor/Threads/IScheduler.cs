using System;
using Viki.LoadRunner.Engine.Executor.Timer;

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public interface IScheduler
    {
        ITimer Timer { get; }

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