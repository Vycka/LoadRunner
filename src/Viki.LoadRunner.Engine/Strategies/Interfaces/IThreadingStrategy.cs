using Viki.LoadRunner.Engine.Executor.Pool.Interfaces;
using Viki.LoadRunner.Engine.Executor.State.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Interfaces
{
    public interface IThreadingStrategy
    {
        void Setup(IThreadPool pool);

        void HeartBeat(IThreadPool pool, ITestState state);
    }
}