using Viki.LoadRunner.Engine.Executor.Strategy.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Strategy.Interfaces
{
    public interface IThreadingContext : IThreadPoolStats
    {
        void StartWorkersAsync(int threadCount);
        void StopWorkersAsync(int threadCount);

        ITimer Timer { get; }
    }
}