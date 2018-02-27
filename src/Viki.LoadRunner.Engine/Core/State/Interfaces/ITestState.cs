using Viki.LoadRunner.Engine.Core.Pool.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Core.State.Interfaces
{
    public interface ITestState : IStrategyState
    {
        int GlobalIterationId { get; }
    }

    public interface IStrategyState
    {
        ITimer Timer { get; }
        IThreadPoolStats ThreadPool { get; }
    }
}