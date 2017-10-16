using Viki.LoadRunner.Engine.Executor.Worker.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Worker
{
    public class NullPrewait : IPrewait
    {
        public void Wait(ref bool stop)
        {
        }
    }
}