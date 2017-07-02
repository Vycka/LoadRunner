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

        public void Next(IThreadContext context, IIterationControl control)
        {
            long timerTicks = context.Timer.Value.Ticks;
            long delta = timerTicks + ScheduleAheadTicks - _next;
            if (delta >= 0)
            {
                long current = Interlocked.Add(ref _next, DelayTicks);
                control.Execute(TimeSpan.FromTicks(current));
            }
            else
            {
                control.Idle(TimeSpan.FromTicks(Math.Abs(delta) + TimeSpan.TicksPerMillisecond));
            }
        }


        public void Adjust(CoordinatorContext context)
        {

        }
    }
}