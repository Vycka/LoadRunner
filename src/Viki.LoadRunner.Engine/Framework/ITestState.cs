using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
using Viki.LoadRunner.Engine.Executor.Timer;

namespace Viki.LoadRunner.Engine.Framework
{
    public interface ITestState
    {
        ITimer Timer { get; }
        int GlobalIterationId { get; }
        IThreadPoolStats ThreadPool { get; }
    }
}