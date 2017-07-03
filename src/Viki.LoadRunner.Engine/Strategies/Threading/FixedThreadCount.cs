using System;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Threading
{
    public class FixedThreadCount : IThreadingStrategy
    {
        private readonly int _threadCount;

        public FixedThreadCount(int threadCount)
        {
            _threadCount = threadCount;
        }

        public void Setup(CoordinatorContext context, IThreadPoolControl control)
        {
            control.StartWorkersAsync(_threadCount);
        }

        public void Adjust(CoordinatorContext context, IThreadPoolControl control)
        {
        }
    }
}