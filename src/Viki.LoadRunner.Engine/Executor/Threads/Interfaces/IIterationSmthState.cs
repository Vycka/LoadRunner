using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Context.Interfaces;

#pragma warning disable 1591

namespace Viki.LoadRunner.Engine.Executor.Threads.Interfaces
{
    public interface IIterationSmthState
    {
        IIterationId Iteration { get; }
        IThreadPoolStats ThreadPool { get; }
    }
}