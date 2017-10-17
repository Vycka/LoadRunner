using Viki.LoadRunner.Engine.Core.Worker.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Worker
{
    public class NullPrewait : IPrewait
    {
        public void Wait(ref bool stop)
        {
        }
    }
}