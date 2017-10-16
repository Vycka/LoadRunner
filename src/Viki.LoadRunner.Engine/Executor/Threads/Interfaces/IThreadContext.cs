using Viki.LoadRunner.Engine.Executor.Context.Interfaces;
using Viki.LoadRunner.Engine.Executor.Timer;

namespace Viki.LoadRunner.Engine.Executor.Threads.Interfaces
{
    public interface IThreadContext
    {
        IThreadPoolStats ThreadPool { get; }
        ITimer Timer { get; }
        IIterationMetadata<object> Iteration { get; }
    }
}