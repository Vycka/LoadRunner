using System;
using System.Linq;
using Viki.LoadRunner.Engine.Executor.Threads;
using Viki.LoadRunner.Engine.Executor.Timer;

namespace Viki.LoadRunner.Engine.Strategies.Speed.PriorityStrategy
{
    internal class PrioritySpeedStrategy : ISpeedStrategy
    {
        private readonly IPriorityResolver _strategy;
        private readonly ISpeedStrategy[] _strategies;
        private readonly ISchedule[] _schedules;

        public PrioritySpeedStrategy(IPriorityResolver strategy, ITimer timer, ISpeedStrategy[] strategies)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));
            if (strategies == null)
                throw new ArgumentNullException(nameof(strategies));
            if (strategies.Length == 0)
                throw new ArgumentException("At least one strategy has to be provided", nameof(strategies));

            _strategy = strategy;
            _strategies = strategies;
            _schedules = Enumerable.Repeat(1,_strategies.Length).Select(i => (ISchedule)new Schedule(timer)).ToArray();
        }

        public void Next(IThreadContext context, ISchedule schedule)
        {
            for (int i = 0; i < _strategies.Length; i++)
            {
                _strategies[i].Next(context, _schedules[i]);
            }

            _strategy.Apply(_schedules, schedule);
        }

        public void HeartBeat(IThreadPoolContext context)
        {
            for (int i = 0; i < _strategies.Length; i++)
            {
                _strategies[i].HeartBeat(context);
            }
        }
    }

    internal interface IPriorityResolver
    {
        void Apply(ISchedule[] schedules, ISchedule target);
    }
}