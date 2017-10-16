using Viki.LoadRunner.Engine.Executor.Pool.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Counter.Interfaces
{
    public interface IThreadPoolCounter : IThreadPoolStats
    {
        void AddIdle(int count);

        void AddInitialized(int count);

        void AddCreated(int count);
    }
}