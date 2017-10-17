using Viki.LoadRunner.Engine.Executor.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Executor.Scheduler.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Factory.Interfaces
{
    public interface ISchedulerFactory
    {
        IScheduler Create(IIterationId iterationContext);
    }
}