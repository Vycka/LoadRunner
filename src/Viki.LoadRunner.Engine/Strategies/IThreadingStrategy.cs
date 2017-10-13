using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
using Viki.LoadRunner.Engine.Framework.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies
{
    public interface IThreadingStrategy
    {
        void Setup(IThreadPool pool);

        void HeartBeat(IThreadPool pool, ITestState state);
    }
}