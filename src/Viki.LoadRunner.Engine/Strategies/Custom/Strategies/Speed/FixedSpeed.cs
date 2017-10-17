using System;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed
{
    public class FixedSpeed : ISpeedStrategy
    {
        protected long ScheduleAheadTicks = TimeSpan.TicksPerSecond;

        private long _delayTicks;
        private long _next;

        public FixedSpeed(double maxIterationsPerSec)
        {
            SetSpeed(maxIterationsPerSec, TimeSpan.Zero);
        }
        
        protected void SetSpeed(double maxIterationsPerSec, TimeSpan timerValue)
        {
            long delayTicks = (long)(TimeSpan.TicksPerSecond / maxIterationsPerSec) + 1;

            SetDelay(delayTicks, timerValue);
        }

        protected void SetDelay(long delayBetweenIterationsTicks, TimeSpan timerValue)
        {
            _delayTicks = delayBetweenIterationsTicks;

            Interlocked.Exchange(ref _next, timerValue.Ticks - _delayTicks);
        }

        public void Next(IIterationState state, ISchedule schedule)
        {
            long timerTicks = state.Timer.Value.Ticks;
            long delta = timerTicks + ScheduleAheadTicks - _next;
            if (delta >= 0)
            {
                long current = Interlocked.Add(ref _next, _delayTicks);
                schedule.ExecuteAt(TimeSpan.FromTicks(current));
            }
            else
            {
                schedule.Idle(TimeSpan.FromTicks(Math.Abs(delta) + TimeSpan.TicksPerMillisecond));
            }
        }

        public void HeartBeat(ITestState state)
        {
            // Catch up _next if lagging behind timeline
            long deltaLag = state.Timer.Value.Ticks - _next;
            long threshold = 2 * _delayTicks;
            if (deltaLag > threshold)
            {
                Interlocked.Add(ref _next, deltaLag - _delayTicks);
            }
        }
    }
}