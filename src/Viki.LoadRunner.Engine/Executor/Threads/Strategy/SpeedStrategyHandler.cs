using System;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
using Viki.LoadRunner.Engine.Strategies;

namespace Viki.LoadRunner.Engine.Executor.Threads.Strategy
{
    public class SpeedStrategyHandler : ISpeedStrategyHandler
    {
        private readonly ISpeedStrategy _strategy;
        private readonly IExecutionState _state;

        public SpeedStrategyHandler(ISpeedStrategy strategy, IIterationId iteration, IThreadPoolStats stats)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            if (iteration == null)
                throw new ArgumentNullException(nameof(iteration));
            if (stats == null)
                throw new ArgumentNullException(nameof(stats));

            _state = new ExecutionState
            {
                Iteration = iteration,
                ThreadPool = stats
            };
                
            _strategy = strategy;
        }

        public void Next(ISchedule schedule)
        {
            // _strategy.Next(_state, schedule);
        }
    }
}