using System;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
using Viki.LoadRunner.Engine.Executor.Timer;

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public class ThreadingContext : IThreadingContext
    {
        public ITimer Timer { get; }

        private readonly IThreadPool _threadPool;
        private readonly IThreadPoolCounter _stats;

        public ThreadingContext(IThreadPool threadPool, IThreadPoolCounter stats, ITimer timer)
        {
            
            if (threadPool == null)
                throw new ArgumentNullException(nameof(threadPool));
            if (stats == null)
                throw new ArgumentNullException(nameof(stats));

            _threadPool = threadPool;
            _stats = stats;

            Timer = timer;
        }

        public void StartWorkersAsync(int count)
        {
            _threadPool.StartWorkersAsync(count);
        }

        public void StopWorkersAsync(int count)
        {
            _threadPool.StopWorkersAsync(count);
        }

        public int CreatedThreadCount => _stats.CreatedThreadCount;
        public int InitializedThreadCount => _stats.InitializedThreadCount;
        public int IdleThreadCount => _stats.IdleThreadCount;
    }
}