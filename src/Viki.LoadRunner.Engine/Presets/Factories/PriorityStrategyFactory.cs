using Viki.LoadRunner.Engine.Executor.Strategy.Timer.Interfaces;
using Viki.LoadRunner.Engine.Presets.Adapters.Speed;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Speed;
using Viki.LoadRunner.Engine.Utils;

namespace Viki.LoadRunner.Engine.Presets.Factories
{
    public static class PriorityStrategyFactory
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