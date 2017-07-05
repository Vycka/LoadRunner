using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies
{
    public interface IThreadingStrategy
    {
        void Setup(IThreadPoolContext context, IThreadPoolControl control);

        void HeartBeat(IThreadPoolContext context, IThreadPoolControl control);
    }
}