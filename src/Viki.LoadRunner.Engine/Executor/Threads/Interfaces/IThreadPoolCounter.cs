namespace Viki.LoadRunner.Engine.Executor.Threads.Interfaces
{
    public interface IThreadPoolCounter : IThreadPoolStats
    {
        void AddIdle(int count);
    }
}