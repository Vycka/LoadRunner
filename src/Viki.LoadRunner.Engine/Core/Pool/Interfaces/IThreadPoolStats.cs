namespace Viki.LoadRunner.Engine.Core.Pool.Interfaces
{
    public interface IThreadPoolStats
    {
        int CreatedThreadCount { get; }
        int InitializedThreadCount { get; }
        int IdleThreadCount { get; }
    }
}