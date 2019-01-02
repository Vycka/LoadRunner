using System.Linq;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Adapter.Speed
{
    public class ScheduleTable
    {
        public ISchedule[] Schedules;
        public int ReadPosition;

        public ScheduleTable(ITimer timer, int size)
        {
            Schedules = Enumerable.Range(0, size).Select(i => new Schedule(timer)).Cast<ISchedule>().ToArray();
            ReadPosition = 0;
        }
    }
}