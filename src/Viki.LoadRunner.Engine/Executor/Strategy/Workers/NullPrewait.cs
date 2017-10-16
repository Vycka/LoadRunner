using Viki.LoadRunner.Engine.Executor.Strategy.Workers.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Strategy.Workers
{
    public class NullPrewait : IPrewait
    {
        public void Wait(ref bool stop)
        {
        }
    }
}