using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Factory
{
    public class NullSchedulerFactory : ISchedulerFactory
    {
        private readonly NullScheduler _scheduler = new NullScheduler();

        public IScheduler Create(IIterationId iterationContext)
        {
            return _scheduler;
        }
    }
}