using System;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
using Viki.LoadRunner.Engine.Framework;
using Viki.LoadRunner.Engine.Strategies;

namespace Viki.LoadRunner.Engine.Executor.Threads.Strategy
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