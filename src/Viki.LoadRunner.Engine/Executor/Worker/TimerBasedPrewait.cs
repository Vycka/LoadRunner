using System;
using System.Threading;
using Viki.LoadRunner.Engine.Executor.Timer.Interfaces;
using Viki.LoadRunner.Engine.Executor.Worker.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Worker
{
    public class TimerBasedPrewait : IPrewait
    {
        private readonly ITimer _timer;

        public TimerBasedPrewait(ITimer timer)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));

            _timer = timer;
        }

        public void Wait(ref bool stop)
        {
            // Wait for ITimer to start.
            while (_timer.IsRunning == false && stop == false)
                Thread.Sleep(1);
        }
    }
}