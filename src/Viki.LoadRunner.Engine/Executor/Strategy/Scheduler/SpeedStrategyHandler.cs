using System;
using Viki.LoadRunner.Engine.Executor.Strategy.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.State.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Strategy.Scheduler
{
    public class SpeedStrategyHandler : ISpeedStrategyHandler
    {
        private readonly ISpeedStrategy _strategy;
        private readonly IIterationState _state;

        public SpeedStrategyHandler(ISpeedStrategy strategy, IIterationState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            _strategy = strategy;
            _state = state;
        }

        public void Next(ISchedule schedule)
        {
             _strategy.Next(_state, schedule);
        }
    }
}