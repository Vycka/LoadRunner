using System;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Context.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
using Viki.LoadRunner.Engine.Executor.Timer;

namespace Viki.LoadRunner.Engine.Framework
{
    public class IterationState : IIterationState
    {
        public ITimer Timer { get; }
        public IIterationId Iteration { get; }
        public IThreadPoolStats ThreadPool { get; }

        public IterationState(ITimer timer, IIterationId iteration, IThreadPoolStats threadPool)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));
            if (iteration == null)
                throw new ArgumentNullException(nameof(iteration));
            if (threadPool == null)
                throw new ArgumentNullException(nameof(threadPool));

            Timer = timer;
            Iteration = iteration;
            ThreadPool = threadPool;
        }
    }
}