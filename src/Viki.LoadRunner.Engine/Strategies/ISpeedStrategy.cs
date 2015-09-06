using System;

namespace Viki.LoadRunner.Engine.Strategies
{
    public interface ISpeedStrategy
    {
        TimeSpan GetDelayBetweenIterations(TimeSpan testExecutionTime);
    }
}