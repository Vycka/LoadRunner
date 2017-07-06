using Viki.LoadRunner.Engine.Executor.Timer;
using Viki.LoadRunner.Engine.Utils;

namespace Viki.LoadRunner.Engine.Strategies.Speed.PriorityStrategy
{
    public static class StrategyBuilder
    {
        public static ISpeedStrategy Create(ISpeedStrategy[] strategies, ITimer timer)
        {
            if (strategies.IsNullOrEmpty())
                return new MaxSpeed();

            if (strategies.Length == 1)
                return strategies[0];


            return new PrioritySpeedStrategy(new SlowestPriority(), timer, strategies);
        } 
    }
}