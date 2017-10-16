using Viki.LoadRunner.Engine.Executor.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Strategy.Interfaces
{
    public interface IThreadContext
    {
        IThreadPoolStats ThreadPool { get; }
        ITimer Timer { get; }
        IIterationMetadata<object> Iteration { get; }
    }
}