using System;
using Viki.LoadRunner.Engine.Core.Counter.Interfaces;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Factory
{
    public class SchedulerFactory : ISchedulerFactory
    {
        private readonly ITimer _timer;
        private readonly ISpeedStrategy _speedStrategy;
        private readonly IThreadPoolCounter _counter;

        public SchedulerFactory(ITimer timer, ISpeedStrategy speedStrategy, IThreadPoolCounter counter)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));
            if (speedStrategy == null)
                throw new ArgumentNullException(nameof(speedStrategy));
            if (counter == null)
                throw new ArgumentNullException(nameof(counter));

            _timer = timer;
            _speedStrategy = speedStrategy;
            _counter = counter;
        }

        public IScheduler Create(IIterationId iterationContext)
        {
            IScheduler scheduler = new Scheduler.Scheduler(_speedStrategy, _counter, _timer, iterationContext);

            return scheduler;
        }
    }
}