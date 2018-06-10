using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Factory
{
    public class NullSchedulerFactory : ISchedulerFactory
    {
        public IScheduler Create(IIterationId iterationContext)
        {
            return new NullScheduler();
        }
    }
}