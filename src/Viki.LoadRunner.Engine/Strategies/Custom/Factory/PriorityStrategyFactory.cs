using Viki.LoadRunner.Engine.Core.Timer.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Adapter.Speed;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed;
using Viki.LoadRunner.Engine.Utils;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Factory
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