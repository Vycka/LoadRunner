using Viki.LoadRunner.Engine.Executor.Pool.Interfaces;
using Viki.LoadRunner.Engine.Executor.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.State.Interfaces
{
    public interface ITestState
    {
        ITimer Timer { get; }
        int GlobalIterationId { get; }
        IThreadPoolStats ThreadPool { get; }
    }
}