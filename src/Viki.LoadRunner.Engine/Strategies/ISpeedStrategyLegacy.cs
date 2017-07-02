using System;

namespace Viki.LoadRunner.Engine.Strategies
{
    public interface ISpeedStrategyLegacy
    {
        TimeSpan GetDelayBetweenIterations(TimeSpan testExecutionTime);
    }
}