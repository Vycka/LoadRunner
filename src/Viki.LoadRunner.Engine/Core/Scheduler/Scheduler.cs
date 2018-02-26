using System;
using Viki.LoadRunner.Engine.Core.Counter.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scheduler
{
    public class Scheduler : IScheduler, ISchedule
    {
        private readonly ISpeedStrategyHandler _strategy;
        private readonly IThreadPoolCounter _counter;

        private readonly IWait _waiter;
        
        public ITimer Timer { get; }

        public Scheduler(ISpeedStrategyHandler strategy, IThreadPoolCounter counter, ITimer timer)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            if (counter == null)
                throw new ArgumentNullException(nameof(counter));

            _strategy = strategy;
            _counter = counter;
            Timer = timer;

            _waiter = new SemiWait(timer);
        }

        public ScheduleAction Action { get; set; } = ScheduleAction.Execute;
        public TimeSpan At { get; set; } = TimeSpan.Zero;

        public void WaitNext(ref bool stop)
        {
            _strategy.Next(this);

            if (Action == ScheduleAction.Idle || At > Timer.Value)
            {
                _counter.AddIdle(1);

                while (Action == ScheduleAction.Idle && stop == false)
                {
                    _waiter.Wait(At, ref stop);
                    _strategy.Next(this);
                }

                _waiter.Wait(At, ref stop);

                _counter.AddIdle(-1);
            }
        }
    }
}