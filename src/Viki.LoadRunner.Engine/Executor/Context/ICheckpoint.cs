using System;

namespace Viki.LoadRunner.Engine.Executor.Context
{
    public interface ICheckpoint
    {
        string Name { get; }
        TimeSpan TimePoint { get; }
        Exception Error { get; }
    }
}