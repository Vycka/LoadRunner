using System;
using Viki.LoadRunner.Engine.Executor.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.State.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Strategy.State
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