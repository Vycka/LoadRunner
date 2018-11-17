using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed
{
    public class MaxSpeed : ISpeedStrategy
    {
        public void Setup(ITestState state)
        {
        }

        public void Next(IIterationId id, ISchedule scheduler)
        {
            scheduler.Execute();
        }

        public void HeartBeat(ITestState state)
        {
        }
    }
}