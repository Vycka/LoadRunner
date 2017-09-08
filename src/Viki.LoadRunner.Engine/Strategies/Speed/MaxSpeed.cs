using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Speed
{
    public class MaxSpeed : ISpeedStrategy
    {
        public void Next(IThreadContextWat context, ISchedule schedule)
        {
            schedule.Execute();
        }

        public void HeartBeat(IThreadPoolContext context)
        {
        }
    }
}