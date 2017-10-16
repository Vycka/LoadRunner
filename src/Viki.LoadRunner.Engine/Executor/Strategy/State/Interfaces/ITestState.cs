using Viki.LoadRunner.Engine.Executor.Strategy.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Strategy.State.Interfaces
{
    public interface ITestState
    {
        ITimer Timer { get; }
        int GlobalIterationId { get; }
        IThreadPoolStats ThreadPool { get; }
    }
}