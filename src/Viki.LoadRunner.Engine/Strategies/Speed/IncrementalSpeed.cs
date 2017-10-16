using System;
using Viki.LoadRunner.Engine.Executor.Strategy.State.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Speed
{
    public class IncrementalSpeed : FixedSpeed, ISpeedStrategy
    {
        private readonly double _initialRequestsPerSec;
        private readonly TimeSpan _increasePeriod;
        private readonly double _increaseStep;

        /// <summary>
        ///  Limits executed iterations per second and incrementally changes the limit in a way defined by parameters
        /// </summary>
        /// <param name="initialRequestsPerSec">initial iterations per second limit</param>
        /// <param name="increasePeriod">Time period for increases</param>
        /// <param name="increaseStep">delta change</param>
        public IncrementalSpeed(double initialRequestsPerSec, TimeSpan increasePeriod, double increaseStep) : base(initialRequestsPerSec)
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

        public new void HeartBeat(ITestState state)
        {
            HeartBeatInner(state.Timer.Value);
            base.HeartBeat(state);
        }

        private int _multiplier = 0;

        private void HeartBeatInner(TimeSpan timerValue)
        {
            int multiplier = (int)(timerValue.Ticks / _increasePeriod.Ticks);
            if (multiplier != _multiplier)
            {
                _multiplier = multiplier;

                double newSpeed = _initialRequestsPerSec + (multiplier * _increaseStep);

                SetSpeed(newSpeed, timerValue);
            }
        }
    }
}