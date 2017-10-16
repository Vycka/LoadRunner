namespace Viki.LoadRunner.Engine.Executor.Strategy.Interfaces
{
    public interface IThreadPoolStats
    {
        int CreatedThreadCount { get; }
        int InitializedThreadCount { get; }
        int IdleThreadCount { get; }
    }
}