using Viki.LoadRunner.Engine.Executor.Timer;
using Viki.LoadRunner.Engine.Utils;

namespace Viki.LoadRunner.Engine.Strategies.Speed.PriorityStrategy
{
    public static class StrategyBuilder
    {
        public static ISpeedStrategy Create(ISpeedStrategy[] strategies, Priority priority, ITimer timer)
        {
            if (strategies.IsNullOrEmpty())
                return new MaxSpeed();

            if (strategies.Length == 1)
                return strategies[0];

            IPriorityResolver resolver = priority == Priority.Fastest ? (IPriorityResolver) new FastestPriority() : new SlowestPriority();

            return new PrioritySpeedStrategy(resolver, timer, strategies);
        } 
    }
}