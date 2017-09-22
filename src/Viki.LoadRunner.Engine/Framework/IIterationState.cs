using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
using Viki.LoadRunner.Engine.Executor.Timer;

namespace Viki.LoadRunner.Engine.Framework
{
    public interface IIterationState
    {
        ITimer Timer { get; }
        IIterationId Iteration { get; }
        IThreadPoolStats ThreadPool { get; }
    }
}