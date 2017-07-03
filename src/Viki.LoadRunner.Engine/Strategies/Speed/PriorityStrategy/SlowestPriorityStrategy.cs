using Viki.LoadRunner.Engine.Executor.Threads;
using Viki.LoadRunner.Engine.Executor.Timer;

namespace Viki.LoadRunner.Engine.Strategies.Speed.PriorityStrategy
{
    public class SlowestPriorityStrategy : Scheduler
    {
        public SlowestPriorityStrategy(ITimer timer) : base(timer)
        {
        }
    }
}