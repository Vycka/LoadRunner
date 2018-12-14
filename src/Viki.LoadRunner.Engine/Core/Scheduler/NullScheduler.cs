using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scheduler
{
    public class NullScheduler : IScheduler
    {
        public void WaitNext(ref bool stop)
        {
        }

        public void ThreadFinished()
        {
        }
    }
}