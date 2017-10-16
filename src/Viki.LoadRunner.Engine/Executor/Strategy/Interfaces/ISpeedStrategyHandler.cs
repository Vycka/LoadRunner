

using Viki.LoadRunner.Engine.Executor.Strategy.Scheduler.Interfaces;

#pragma warning disable 1591

namespace Viki.LoadRunner.Engine.Executor.Strategy.Interfaces
{
    public interface ISpeedStrategyHandler
    {
        void Next(ISchedule schedule);
    }
}