namespace Viki.LoadRunner.Engine.Executor.Threads.Interfaces
{
    public interface IThreadPoolCounter : IThreadPoolStats
    {
        void AddIdle(int count);

        void AddInitialized(int count);

        void AddCreated(int count);
    }
}