using System;
using Viki.LoadRunner.Engine.Core.Counter.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scheduler
{
    public class Scheduler : IScheduler, ISchedule
    {
        private readonly ISpeedStrategy _strategy;
        private readonly IThreadPoolCounter _counter;
        private readonly IIterationId _id;

        private readonly IWait _waiter;
        
        public ITimer Timer { get; }

        public Scheduler(ISpeedStrategy strategy, IThreadPoolCounter counter, ITimer timer, IIterationId id)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            if (counter == null)
                throw new ArgumentNullException(nameof(counter));

            _strategy = strategy;
            _counter = counter;
            _id = id ?? throw new ArgumentNullException(nameof(id));

            Timer = timer;

            _waiter = new SemiWait(Timer);
        }

        public ScheduleAction Action { get; set; } = ScheduleAction.Execute;
        public TimeSpan At { get; set; } = TimeSpan.Zero;

        public bool WaitForSchedule(ref bool stop)
        {
            _strategy.Next(_id, this);

            if (Action == ScheduleAction.Idle || At > Timer.Value)
            {
                _counter.AddIdle(1);

                while (Action == ScheduleAction.Idle && stop == false)
                {
                    _waiter.Wait(At, ref stop);
                    _strategy.Next(_id, this);
                }

                return true;
            }

            return false;
        }

        public void Wait(ref bool stop)
        {
            _waiter.Wait(At, ref stop);

            _counter.AddIdle(-1);
        }

        public void ThreadStarted()
        {
            _strategy.ThreadStarted(_id, this);
        }

        public void ThreadFinished()
        {
            _strategy.ThreadFinished(_id, this);
        }
    }
}