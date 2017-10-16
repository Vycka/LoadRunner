using Viki.LoadRunner.Engine.Executor.Timer.Interfaces;
using Viki.LoadRunner.Engine.Presets.Adapter.Speed;
using Viki.LoadRunner.Engine.Strategies.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Speed;
using Viki.LoadRunner.Engine.Utils;

namespace Viki.LoadRunner.Engine.Presets.Factory
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