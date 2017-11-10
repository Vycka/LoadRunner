﻿using System;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Counter.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scheduler
{
    public class Scheduler : IScheduler, ISchedule
    {
        private readonly ISpeedStrategyHandler _strategy;
        private readonly IThreadPoolCounter _counter;

        private readonly TimeSpan _oneSecond = TimeSpan.FromSeconds(1);

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
        }

        public ScheduleAction Action { get; set; } = ScheduleAction.Execute;
        public TimeSpan At { get; set; } = TimeSpan.Zero;


        public void WaitNext(ref bool stop)
        {
            _strategy.Next(this);

            TimeSpan delay = CalculateDelay();
            if (Action == ScheduleAction.Idle || delay > TimeSpan.Zero)
            {
                _counter.AddIdle(1);

                while (Action == ScheduleAction.Idle && stop == false)
                {
                    SemiWait(delay, ref stop);

                    _strategy.Next(this);
                }

                SemiWait(delay, ref stop);

                _counter.AddIdle(-1);
            }
        }

        private TimeSpan CalculateDelay()
        {
            return At - Timer.Value;
        }


        private void SemiWait(TimeSpan delay, ref bool stop)
        {
            while (delay > _oneSecond && stop == false)
            {
                Thread.Sleep(_oneSecond);
                delay = CalculateDelay();
            }
            if (delay > TimeSpan.Zero && stop == false)
                Thread.Sleep(delay);
        }
    }
}