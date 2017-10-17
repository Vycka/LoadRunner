using System;
using Viki.LoadRunner.Engine.Executor.Counter.Interfaces;
using Viki.LoadRunner.Engine.Executor.Factory.Interfaces;
using Viki.LoadRunner.Engine.Executor.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Executor.Scheduler;
using Viki.LoadRunner.Engine.Executor.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Executor.State;
using Viki.LoadRunner.Engine.Executor.State.Interfaces;
using Viki.LoadRunner.Engine.Executor.Timer.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Factory
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
            IIterationState iterationState = new IterationState(_timer, iterationContext, _counter);
            SpeedStrategyHandler strategyHandler = new SpeedStrategyHandler(_speedStrategy, iterationState);

            IScheduler scheduler = new Scheduler.Scheduler(strategyHandler, _counter, _timer);

            return scheduler;
        }
    }
}