using System;
using System.Threading;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Speed
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
        
        public void SetSpeed(double maxIterationsPerSec, TimeSpan timerValue)
        {
            long delayTicks = (long)(TimeSpan.TicksPerSecond / maxIterationsPerSec) + 1;

            SetDelay(delayTicks, timerValue);
        }

        public void SetDelay(long delayTicks, TimeSpan timerValue)
        {
            _delayTicks = delayTicks;

            Interlocked.Exchange(ref _next, timerValue.Ticks - _delayTicks);
        }

        public void Next(IThreadContext context, IScheduler scheduler)
        {
            long timerTicks = context.Timer.Value.Ticks;
            long deltaTimeline = timerTicks + ScheduleAheadTicks - _next;
            if (deltaTimeline >= 0)
            {
                long current = Interlocked.Add(ref _next, _delayTicks);
                scheduler.ExecuteAt(TimeSpan.FromTicks(current));
            }
            else
            {
                scheduler.Idle(TimeSpan.FromTicks(Math.Abs(deltaTimeline) + TimeSpan.TicksPerMillisecond));
            }
        }

        public void Adjust(IThreadPoolContext context)
        {
            // Catch up _next if lagging behind timeline
            long deltaLag = context.Timer.Value.Ticks - _next;
            long threshold = 2 * _delayTicks;
            if (deltaLag > threshold)
            {
                Interlocked.Add(ref _next, deltaLag - _delayTicks);
            }
        }
    }
}