using Viki.LoadRunner.Engine.Executor.Threads.Scheduler.Interfaces;

namespace Viki.LoadRunner.Engine.Presets.Strategies.Speed
{
    public class SlowestPriority : IPriorityResolver
    {
        public void Apply(ISchedule[] schedules, ISchedule target)
        {
            target.Action = schedules[0].Action;
            target.At = schedules[0].At;

            for (int i = 1; i < schedules.Length; i++)
            {
                ISchedule schedule = schedules[i];

                if (target.Action == ScheduleAction.Idle)
                {
                    if (schedule.Action == ScheduleAction.Idle)
                        if (target.At < schedule.At)
                            target.At = schedule.At;
                }
                else
                {
                    if (schedule.Action == ScheduleAction.Execute)
                    {
                        if (target.At < schedule.At)
                            target.At = schedule.At;
                    }
                    else
                    {
                        target.Action = ScheduleAction.Idle;
                        target.At = schedule.At;
                    }
                }
            }
        }
    }
}