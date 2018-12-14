using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces
{
    public interface ISpeedStrategy
    {
        void Setup(ITestState state);

        void Next(IIterationId id, ISchedule scheduler); // Must be thread safe

        void HeartBeat(ITestState state);

        void ThreadFinished(IIterationId id, ISchedule scheduler);
    }
}