using Viki.LoadRunner.Engine.Executor.Threads.Workers.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Threads.Workers
{
    public class NullPrewait : IPrewait
    {
        public void Wait(ref bool stop)
        {
        }
    }
}