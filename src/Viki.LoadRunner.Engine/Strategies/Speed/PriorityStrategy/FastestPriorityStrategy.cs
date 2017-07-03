using System;
using Viki.LoadRunner.Engine.Executor.Threads;
using Viki.LoadRunner.Engine.Executor.Timer;

namespace Viki.LoadRunner.Engine.Strategies.Speed.PriorityStrategy
{
    public class FastestPriorityStrategy : IScheduler
    {
        public ITimer Timer { get; }
        public ScheduleAction Action { get; }
        public TimeSpan At { get; }

        public void Reset()
        {
            
        }

        public void Idle(TimeSpan delay)
        {
            throw new NotImplementedException();
        }

        public void ExecuteAt(TimeSpan at)
        {
            throw new NotImplementedException();
        }
    }
}