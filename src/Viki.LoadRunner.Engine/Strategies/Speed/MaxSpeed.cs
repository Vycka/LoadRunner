using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Speed
{
    public class MaxSpeed : ISpeedStrategy
    {
        public void Next(IThreadContext context, ISchedule schedule)
        {
            schedule.Execute();
        }

        public void HeartBeat(IThreadPoolContext context)
        {
        }
    }
}