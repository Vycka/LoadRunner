using Viki.LoadRunner.Engine.Executor.Pool.Interfaces;
using Viki.LoadRunner.Engine.Executor.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Executor.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.State.Interfaces
{
    public interface IIterationState
    {
        ITimer Timer { get; }
        IIterationId Iteration { get; }
        IThreadPoolStats ThreadPool { get; }
    }
}