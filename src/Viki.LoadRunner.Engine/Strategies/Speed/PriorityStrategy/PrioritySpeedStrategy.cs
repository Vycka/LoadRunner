using System;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Speed.PriorityStrategy
{
    internal class PrioritySpeedStrategy : ISpeedStrategy
    {
        //private readonly PriorityStrategy _strategy;
        private readonly ISpeedStrategy[] _strategies;

        public PrioritySpeedStrategy(Priority strategy, ISpeedStrategy[] strategies)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            if (strategies == null)
                throw new ArgumentNullException(nameof(strategies));
            if (strategies.Length == 0)
                throw new ArgumentException("At least one strategy has to be provided", nameof(strategies));

            //_strategy = strategy;
            _strategies = strategies;
        }

        // TODO:
        public void Next(IThreadContext context, ISchedule schedule)
        {
            throw new System.NotImplementedException();
        }

        public void HeartBeat(IThreadPoolContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}