using Viki.LoadRunner.Engine.Executor.Strategy.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.State.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Interfaces;

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