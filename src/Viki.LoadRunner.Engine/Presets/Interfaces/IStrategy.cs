using Viki.LoadRunner.Engine.Executor.Threads.Counters.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Factory.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;

namespace Viki.LoadRunner.Engine.Presets.Interfaces
{
    public interface IStrategy
    {
        void Initialize(IThreadPoolCounter counter);

        void Start(IThreadPool pool);

        bool HeartBeat();

        void Stop();

        // GlobalIdCounter probably can be created in settings, because settings(strategy)  could control all parts of LoadRunnerEngine where GlobalIdCounter is needed.
        IWorkerThreadFactory CreateWorkerThreadFactory();

        //IExecutionStrategy CreateExecutionStrategy(ITimer timer, IThreadPoolCounter counter);
    }
}