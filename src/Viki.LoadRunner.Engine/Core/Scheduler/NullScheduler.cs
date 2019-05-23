using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scheduler
{
    public class NullScheduler : IScheduler
    {
        public void ThreadStarted()
        {
        }

        public bool WaitForSchedule(ref bool stop)
        {
            return false;
        }

        public void Wait(ref bool stop)
        {
        }

        public void ThreadFinished()
        {
        }
    }
}