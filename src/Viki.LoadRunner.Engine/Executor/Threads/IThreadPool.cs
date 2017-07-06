namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public interface IThreadPool : IThreadPoolStats, IThreadPoolControl, IThreadPoolCounter
    {   
    }

    public interface IThreadPoolCounter
    {
        void AddIdle(int count);
    }
}