using Viki.LoadRunner.Engine.Core.Pool.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Core.State.Interfaces
{
    public interface IIterationState
    {
        ITimer Timer { get; }
        IIterationId Iteration { get; }
        IThreadPoolStats ThreadPool { get; }
    }
}