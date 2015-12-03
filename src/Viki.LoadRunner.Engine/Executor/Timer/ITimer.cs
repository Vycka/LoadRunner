using System;

namespace Viki.LoadRunner.Engine.Executor.Timer
{
    public interface ITimer
    {
        TimeSpan CurrentValue { get; }
        DateTime BeginTime { get; }
    }
}