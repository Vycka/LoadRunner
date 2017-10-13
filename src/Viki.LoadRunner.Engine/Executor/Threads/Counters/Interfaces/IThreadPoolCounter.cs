using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Threads.Counters.Interfaces
{
    public interface IThreadPoolCounter : IThreadPoolStats
    {
        void AddIdle(int count);

        void AddInitialized(int count);

        void AddCreated(int count);
    }
}