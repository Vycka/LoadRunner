using Viki.LoadRunner.Engine.Executor.Strategy.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.State.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Interfaces
{
    public interface ISpeedStrategy
    {
        void Next(IIterationState state, ISchedule scheduler); // Must be thread safe

        void HeartBeat(ITestState state);
    }
}