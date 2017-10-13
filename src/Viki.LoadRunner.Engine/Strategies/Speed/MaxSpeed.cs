using Viki.LoadRunner.Engine.Executor.Threads.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Framework;
using Viki.LoadRunner.Engine.Framework.Interfaces;

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