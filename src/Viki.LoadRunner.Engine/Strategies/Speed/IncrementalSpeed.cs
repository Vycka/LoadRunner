using System;

namespace Viki.LoadRunner.Engine.Strategies.Speed
{
    public class IncrementalSpeed : ISpeedStrategy
    {
        private readonly double _initialRequestsPerSec;
        private readonly TimeSpan _increasePeriod;
        private readonly double _increaseStep;

        public IncrementalSpeed(double initialRequestsPerSec, TimeSpan increasePeriod, double increaseStep)
        {
            _initialRequestsPerSec = initialRequestsPerSec;
            _increasePeriod = increasePeriod;
            _increaseStep = increaseStep;
        }

        public TimeSpan GetDelayBetweenIterations(TimeSpan testExecutionTime)
        {
            double currentRequestsPerSec = _initialRequestsPerSec + ((long)(testExecutionTime.Ticks / _increasePeriod.Ticks) * _increaseStep);

            TimeSpan delay = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / currentRequestsPerSec));

            return delay;
        }
    }
}