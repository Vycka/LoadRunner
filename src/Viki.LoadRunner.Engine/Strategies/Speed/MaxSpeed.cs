using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Speed
{
    public class MaxSpeed : ISpeedStrategy
    {
        public void Next(IThreadContext context, IScheduler scheduler)
        {
            scheduler.Execute();
        }

        public void Adjust(IThreadPoolContext context)
        {
        }
    }
}