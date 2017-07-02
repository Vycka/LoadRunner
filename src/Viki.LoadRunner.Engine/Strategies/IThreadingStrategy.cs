using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies
{
    public interface IThreadingStrategy
    {
        void Setup(CoordinatorContext context, IThreadPoolControl control);

        void Adjust(CoordinatorContext context, IThreadPoolControl control);
    }
}