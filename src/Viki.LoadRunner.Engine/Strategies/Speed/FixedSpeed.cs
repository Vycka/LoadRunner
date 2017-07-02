using System;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Speed
{
    public class FixedSpeed : ISpeedStrategyLegacy, ISpeedStrategy
    {
        private readonly TimeSpan _delay;
        private readonly long _delayTicks;

        public FixedSpeed(double maxIterationsPerSec = Double.MaxValue)
        {
            long delayTicks = (long)(TimeSpan.TicksPerSecond / maxIterationsPerSec);

            _delay = TimeSpan.FromTicks(delayTicks);
            _delayTicks = _delay.Ticks;
        }

        public TimeSpan GetDelayBetweenIterations(TimeSpan testExecutionTime)
        {
            return _delay;
        }

        public TimeSpan Next(IThreadContext context)
        {
            return TimeSpan.FromTicks(_delayTicks * context.Iteration.GlobalIterationId);
        }
    }
}