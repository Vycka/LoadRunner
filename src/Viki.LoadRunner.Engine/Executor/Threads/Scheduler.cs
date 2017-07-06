using System;
using System.Threading;
using Viki.LoadRunner.Engine.Executor.Timer;
using Viki.LoadRunner.Engine.Strategies;

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public class Scheduler : ISchedule
    {
        private readonly ISpeedStrategy _strategy;
        private readonly IThreadContext _context;
        private readonly IThreadPoolCounter _counter;

        private readonly TimeSpan _oneSecond = TimeSpan.FromSeconds(1);

        public ITimer Timer { get; }

        public Scheduler(ISpeedStrategy strategy, IThreadContext context, IThreadPoolCounter counter)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (counter == null)
                throw new ArgumentNullException(nameof(counter));

            _strategy = strategy;
            _context = context;
            _counter = counter;
            Timer = context.Timer;
        }

        public ScheduleAction Action { get; set; } = ScheduleAction.Execute; 
        public TimeSpan At { get; set; } = TimeSpan.Zero;

        public virtual void Idle(TimeSpan delay)
        {
            At = Timer.Value + delay;

            Action = ScheduleAction.Idle;
        }

        public virtual void ExecuteAt(TimeSpan at)
        {
            At = at;

            Action = ScheduleAction.Execute;
        }

        public TimeSpan CalculateDelay()
        {
            return At - Timer.Value;
        }

        public bool Execute(ref bool cancellationToken)
        {
            _strategy.Next(_context, this);

            TimeSpan delay = CalculateDelay();
            if (delay > TimeSpan.Zero)
            {
                _counter.AddIdle(1);

                if (Action == ScheduleAction.Idle)
                {
                    while (cancellationToken == false && Action == ScheduleAction.Idle)
                    {
                        while (delay > _oneSecond && cancellationToken == false)
                        {
                            Thread.Sleep(_oneSecond);
                            delay = CalculateDelay();
                        }
                        if (delay > TimeSpan.Zero && cancellationToken == false)
                            Thread.Sleep(delay);

                        _strategy.Next(_context, this);
                    }
                }

                Thread.Sleep(delay);

                _counter.AddIdle(-1);
            }

            return cancellationToken;
        }
    }
}