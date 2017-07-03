using System;
using System.Threading;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Speed
{
    public class FixedSpeed : ISpeedStrategy
    {
        protected long ScheduleAheadTicks = TimeSpan.TicksPerSecond;

        protected long DelayTicks;

        private long _next = 0;

        public FixedSpeed(double maxIterationsPerSec = Double.MaxValue)
        {
            SetSpeed(maxIterationsPerSec);
        }
        
        public void SetSpeed(double maxIterationsPerSec)
        {
            long delayTicks = (long)(TimeSpan.TicksPerSecond / maxIterationsPerSec) + 1;

            DelayTicks = delayTicks;
        }

        public void SetSpeed(TimeSpan delay)
        {
            DelayTicks = delay.Ticks;
        }

        public void Next(IThreadContext context, IIterationControl control)
        {
            long timerTicks = context.Timer.Value.Ticks;
            long deltaTimeline = timerTicks + ScheduleAheadTicks - _next;
            if (deltaTimeline >= 0)
            {
                long current = Interlocked.Add(ref _next, DelayTicks);
                control.Execute(TimeSpan.FromTicks(current));
            }
            else
            {
                control.Idle(TimeSpan.FromTicks(Math.Abs(deltaTimeline) + TimeSpan.TicksPerMillisecond));
            }
        }

        public void Adjust(CoordinatorContext context)
        {
            // Catch up _next if lagging behind timeline
            long deltaLag = context.Timer.Value.Ticks - _next;
            long threshold = 2 * DelayTicks;
            if (deltaLag > threshold)
            {
                Interlocked.Add(ref _next, deltaLag - DelayTicks);
            }
        }
    }
}