namespace Viki.LoadRunner.Engine.Strategies.Custom.Adapter.Speed
{
    // This is fundamentally broken, no idea how to implement it properly.
    //

    //public class FastestPriority : IPriorityResolver
    //{
    //    public void Apply(ISchedule[] schedules, ISchedule target)
    //    {
    //        target.Action = schedules[0].Action;
    //        target.At = schedules[0].At;

    //        for (int i = 1; i < schedules.Length; i++)
    //        {
    //            ISchedule schedule = schedules[i];

    //            if (target.Action == ScheduleAction.Execute)
    //            {
    //                if (schedule.Action == ScheduleAction.Execute)
    //                    if (target.At > schedule.At)
    //                        target.At = schedule.At;
    //            }
    //            else
    //            {
    //                if (schedule.Action == ScheduleAction.Idle)
    //                {
    //                    if (target.At > schedule.At)
    //                        target.At = schedule.At;
    //                }
    //                else
    //                {
    //                    target.Action = ScheduleAction.Execute;
    //                    target.At = schedule.At;
    //                }
    //            }
    //        }

    //        if (target.Action == ScheduleAction.Execute)
    //        {
    //            // Invalidate all other pending Executes in non-intrusive way 
    //            // Other then that, Only slowest cap works, no idea how to nicely implement Fastest version
    //            // But fastest one probably isint needed, dunno any case where Fastest would be needed anyway.
    //            //
    //            // Catch state, invalidate others, adjust other IShedule's 
    //        }
    //    }
    //}
}