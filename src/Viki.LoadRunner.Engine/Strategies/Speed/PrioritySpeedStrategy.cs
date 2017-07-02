using System;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Speed
{
    public enum Priority
    {
        Slowest,
        Fastest
    }
    internal class PrioritySpeedStrategy : ISpeedStrategy
    {
        private readonly Priority _priority;
        private readonly ISpeedStrategy[] _strategies;

        public PrioritySpeedStrategy(Priority priority, ISpeedStrategy[] strategies)
        {
            if (strategies == null)
                throw new ArgumentNullException(nameof(strategies));
            if (strategies.Length == 0)
                throw new ArgumentException("At least one strategy has to be provided", nameof(strategies));

            _priority = priority;
            _strategies = strategies;
        }

        // TODO:
        public void Next(IThreadContext context, IIterationControl control)
        {
            throw new System.NotImplementedException();
        }

        public void Adjust(CoordinatorContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}