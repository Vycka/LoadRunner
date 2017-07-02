namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public interface IThreadPoolStats
    {
        int CreatedThreadCount { get; }
        int InitializedThreadCount { get; }
        //int ReadyThreadCount { get; }
        //int WorkingThreadCount { get; }
    }
}