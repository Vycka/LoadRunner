using Viki.LoadRunner.Engine.Executor.Context;

#pragma warning disable 1591

namespace Viki.LoadRunner.Engine.Executor.Threads.Interfaces
{
    public interface IIterationSmthState
    {
        IIterationId Iteration { get; }
        IThreadPoolStats ThreadPool { get; }
    }
}