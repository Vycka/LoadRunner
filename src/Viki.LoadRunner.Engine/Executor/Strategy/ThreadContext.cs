using System;
using Viki.LoadRunner.Engine.Executor.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Strategy
{
    public class ThreadContext : IThreadContext
    {
        public ThreadContext(IThreadPoolStats threadPool, ITimer timer, IIterationMetadata<object> iteration)
        {
            if (threadPool == null)
                throw new ArgumentNullException(nameof(threadPool));
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));
            if (iteration == null)
                throw new ArgumentNullException(nameof(iteration));

            ThreadPool = threadPool;
            Timer = timer;
            Iteration = iteration;
        }

        public IThreadPoolStats ThreadPool { get; }
        public ITimer Timer { get; }
        public IIterationMetadata<object> Iteration { get; }
    }
}