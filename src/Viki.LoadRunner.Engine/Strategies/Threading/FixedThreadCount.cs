using System;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Threading
{
    public class FixedThreadCount : IThreadingStrategy
    {
        public int ThreadCount;

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