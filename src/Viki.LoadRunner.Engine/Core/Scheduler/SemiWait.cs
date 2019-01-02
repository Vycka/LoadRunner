using System;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scheduler
{
    public class SemiWait : IWait
    {
        private readonly TimeSpan _oneSecond = TimeSpan.FromSeconds(1);

        private readonly ITimer _timer;

        public SemiWait(ITimer timer)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));

            _timer = timer;
        }

        public void Wait(TimeSpan target, ref bool stop)
        {
            TimeSpan delay = CalculateDelay(target);
            while (delay > _oneSecond && stop == false)
            {
                Thread.Sleep(_oneSecond);
                delay = CalculateDelay(target);
            }
            if (delay > TimeSpan.Zero && stop == false)
                Thread.Sleep(delay);
        }

        private TimeSpan CalculateDelay(TimeSpan target)
        {
            return target - _timer.Value;
        }
    }
}