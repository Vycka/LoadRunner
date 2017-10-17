using Viki.LoadRunner.Engine.Core.Pool.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Core.State.Interfaces
{
    public interface ITestState
    {
        ITimer Timer { get; }
        int GlobalIterationId { get; }
        IThreadPoolStats ThreadPool { get; }
    }
}