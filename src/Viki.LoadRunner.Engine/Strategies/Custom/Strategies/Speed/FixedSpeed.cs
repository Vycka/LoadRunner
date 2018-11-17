using System;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed
{
    public class FixedSpeed : ISpeedStrategy
    {
        protected long ScheduleAheadTicks = TimeSpan.TicksPerSecond;
        protected long MinCatchupLagTicks = TimeSpan.FromSeconds(2).Ticks;

        private long _delayTicks;
        private long _next;

        private ITimer _timer;

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

        public void Setup(ITestState state)
        {
            _timer = state.Timer;

            _next = -_delayTicks;
        }

        public void Next(IIterationId id, ISchedule schedule)
        {
            long timerTicks = _timer.Value.Ticks;
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
            if (!state.Timer.IsRunning)
                _next = 0;

            // Catch up _next if lagging behind timeline
            long deltaLag = state.Timer.Value.Ticks - _next;
            long threshold = 2 * _delayTicks;
            if (deltaLag > threshold && deltaLag > MinCatchupLagTicks)
            {
                Interlocked.Add(ref _next, deltaLag - _delayTicks);
            }
        }
    }
}