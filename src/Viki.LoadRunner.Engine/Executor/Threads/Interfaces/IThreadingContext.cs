using Viki.LoadRunner.Engine.Executor.Timer;

namespace Viki.LoadRunner.Engine.Executor.Threads.Interfaces
{

    // TODO: Handle no threads, try delete  threads from lowest index
    // due to how ISpeedStrategy by working thread count will look like
    public interface IThreadingContext : IThreadPoolStats
    {
        void StartWorkersAsync(int threadCount);
        void StopWorkersAsync(int threadCount);

        ITimer Timer { get; }
    }
}