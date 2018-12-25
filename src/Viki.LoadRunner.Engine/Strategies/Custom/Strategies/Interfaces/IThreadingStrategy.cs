using Viki.LoadRunner.Engine.Core.Pool.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces
{
    public interface IThreadingStrategy
    {
        /// <summary>
        /// Will be called within load-test initialization. This is a good place to tell how many worker-threads to create before starting the test
        /// </summary>
        /// <param name="pool">ThreadPool controller interface</param>
        void Setup(IThreadPool pool);

        /// <summary>
        /// IStrategyExecutor fires HeartBeat from its own root thread.
        /// This can be used to adjust thread count t
        /// </summary>
        /// <param name="pool">ThreadPool controller interface</param>
        /// <param name="state">Global test state instance which is used throughout the whole test</param>
        void HeartBeat(IThreadPool pool, ITestState state);
    }
}