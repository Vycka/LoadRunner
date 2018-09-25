using System;
using Viki.LoadRunner.Engine.Core.Counter.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scheduler
{
    public class Scheduler : IScheduler, ISchedule
    {
        private readonly ISpeedStrategy _strategy;
        private readonly IThreadPoolCounter _counter;
        private readonly IIterationState _state;

        private readonly IWait _waiter;
        
        public ITimer Timer { get; }

        public Scheduler(ISpeedStrategy strategy, IThreadPoolCounter counter, IIterationState state)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            if (counter == null)
                throw new ArgumentNullException(nameof(counter));

            _strategy = strategy;
            _counter = counter;
            this._state = state ?? throw new ArgumentNullException(nameof(_state));

            Timer = state.Timer;

            _waiter = new SemiWait(state.Timer);
        }

        public ScheduleAction Action { get; set; } = ScheduleAction.Execute;
        public TimeSpan At { get; set; } = TimeSpan.Zero;

        public void WaitNext(ref bool stop)
        {
            _strategy.Next(_state, this);

            if (Action == ScheduleAction.Idle || At > Timer.Value)
            {
                _counter.AddIdle(1);

                while (Action == ScheduleAction.Idle && stop == false)
                {
                    _waiter.Wait(At, ref stop);
                    _strategy.Next(_state, this);
                }

                _waiter.Wait(At, ref stop);

                _counter.AddIdle(-1);
            }
        }
    }
}