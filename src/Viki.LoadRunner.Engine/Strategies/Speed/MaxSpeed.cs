using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Speed
{
    public class MaxSpeed : ISpeedStrategy
    {
        public void Next(IThreadContext context, IIterationControl control)
        {
            control.Execute();
        }

        public void Adjust(CoordinatorContext context)
        {
        }
    }
}