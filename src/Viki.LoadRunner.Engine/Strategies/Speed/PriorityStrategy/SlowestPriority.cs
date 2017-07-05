using Viki.LoadRunner.Engine.Executor.Threads;
using Viki.LoadRunner.Engine.Executor.Timer;

namespace Viki.LoadRunner.Engine.Strategies.Speed.PriorityStrategy
{
    public class SlowestPriority : IPriorityResolver
    {
        public void Apply(ISchedule[] schedules, ISchedule target)
        {
            target.Action = ScheduleAction.Execute;

            for (int i = 0; i < schedules.Length; i++)
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