using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
using Viki.LoadRunner.Engine.Framework;

namespace Viki.LoadRunner.Engine.Strategies.Speed
{
    public class MaxSpeed : ISpeedStrategy
    {
        public void Next(IIterationState state, ISchedule scheduler)
        {
            scheduler.Execute();
        }

        public void HeartBeat(ITestState state)
        {
        }
    }
}