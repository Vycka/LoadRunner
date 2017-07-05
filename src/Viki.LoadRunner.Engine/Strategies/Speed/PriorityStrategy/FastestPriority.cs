using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Speed.PriorityStrategy
{
    public class FastestPriority : IPriorityResolver
    {
        public void Apply(ISchedule[] schedules, ISchedule target)
        {
            target.Action = schedules[0].Action;
            target.At = schedules[0].At;

            for (int i = 1; i < schedules.Length; i++)
            {
                ISchedule schedule = schedules[i];

                if (target.Action == ScheduleAction.Execute)
                {
                    if (schedule.Action == ScheduleAction.Execute)
                        if (target.At > schedule.At)
                            target.At = schedule.At;
                }
                else
                {
                    if (schedule.Action == ScheduleAction.Idle)
                    {
                        if (target.At > schedule.At)
                            target.At = schedule.At;
                    }
                    else
                    {
                        target.Action = ScheduleAction.Execute;
                        target.At = schedule.At;
                    }
                }
            }
        }
    }
}