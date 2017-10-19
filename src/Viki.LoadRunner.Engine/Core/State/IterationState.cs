using System;
using Viki.LoadRunner.Engine.Core.Pool.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Core.State
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