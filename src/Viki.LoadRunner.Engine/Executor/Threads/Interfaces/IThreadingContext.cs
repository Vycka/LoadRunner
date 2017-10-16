using Viki.LoadRunner.Engine.Executor.Timer;

namespace Viki.LoadRunner.Engine.Executor.Threads.Interfaces
{
    public interface IThreadingContext : IThreadPoolStats
    {
        void StartWorkersAsync(int threadCount);
        void StopWorkersAsync(int threadCount);

        ITimer Timer { get; }
    }
}