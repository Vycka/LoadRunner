using Viki.LoadRunner.Engine.Executor.Strategy.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Strategy.Counters.Interfaces
{
    public interface IThreadPoolCounter : IThreadPoolStats
    {
        void AddIdle(int count);

        void AddInitialized(int count);

        void AddCreated(int count);
    }
}