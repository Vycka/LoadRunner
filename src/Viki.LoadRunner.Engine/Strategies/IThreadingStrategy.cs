using System;

namespace Viki.LoadRunner.Engine.Strategies
{
    public interface IThreadingStrategy
    {
        int InitialThreadCount { get; }

        int ThreadCreateBatchSize { get; }

        int GetAllowedThreadCount(TimeSpan testExecutionTime);
    }
}