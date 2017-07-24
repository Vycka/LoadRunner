namespace Viki.LoadRunner.Engine.Executor.Threads.Interfaces
{
    public interface IThreadPoolStats
    {
        int CreatedThreadCount { get; }
        int InitializedThreadCount { get; }
        int IdleThreadCount { get; }
    }
}