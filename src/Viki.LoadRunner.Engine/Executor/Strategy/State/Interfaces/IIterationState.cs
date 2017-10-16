using Viki.LoadRunner.Engine.Executor.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Strategy.State.Interfaces
{
    public interface IIterationState
    {
        ITimer Timer { get; }
        IIterationId Iteration { get; }
        IThreadPoolStats ThreadPool { get; }
    }
}