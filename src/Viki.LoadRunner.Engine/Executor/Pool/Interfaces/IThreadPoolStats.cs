namespace Viki.LoadRunner.Engine.Executor.Pool.Interfaces
{
    public interface IThreadPoolStats
    {
        int CreatedThreadCount { get; }
        int InitializedThreadCount { get; }
        int IdleThreadCount { get; }
    }
}