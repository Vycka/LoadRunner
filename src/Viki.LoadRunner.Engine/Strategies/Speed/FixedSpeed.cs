using System;

namespace Viki.LoadRunner.Engine.Strategies.Speed
{
    public class FixedSpeed : ISpeedStrategy
    {
        private TimeSpan _delay;

        public FixedSpeed(double maxIterationsPerSec = Double.MaxValue)
        {
            long delayTicks = (long)(TimeSpan.TicksPerSecond / maxIterationsPerSec);

            _delay = TimeSpan.FromTicks(delayTicks);
        }

        public TimeSpan GetDelayBetweenIterations(TimeSpan testExecutionTime)
        {
            return _delay;
        }
    }
}