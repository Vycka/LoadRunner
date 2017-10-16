using Viki.LoadRunner.Engine.Executor.Strategy.Pool.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.State.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Interfaces
{
    public interface IThreadingStrategy
    {
        void Setup(IThreadPool pool);

        void HeartBeat(IThreadPool pool, ITestState state);
    }
}