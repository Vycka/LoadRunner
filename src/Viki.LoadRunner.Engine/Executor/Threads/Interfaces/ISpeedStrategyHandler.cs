

using Viki.LoadRunner.Engine.Executor.Threads.Scheduler.Interfaces;

#pragma warning disable 1591

namespace Viki.LoadRunner.Engine.Executor.Threads.Interfaces
{
    public interface ISpeedStrategyHandler
    {
        void Next(ISchedule schedule);
    }
}