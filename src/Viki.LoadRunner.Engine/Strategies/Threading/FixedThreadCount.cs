using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Threading
{
    public class FixedThreadCount : IThreadingStrategy
    {
        protected int ThreadCount;

        public FixedThreadCount(int threadCount)
        {
            ThreadCount = threadCount;
        }

        public void Setup(IThreadPoolContext context, IThreadPoolControl control)
        {
            control.StartWorkersAsync(ThreadCount);
        }

        public void HeartBeat(IThreadPoolContext context, IThreadPoolControl control)
        {
            control.SetWorkerCountAsync(ThreadCount);
        }
    }
}