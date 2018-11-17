using System;
using Viki.LoadRunner.Engine.Core.Pool.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Core.State
{
    public class TestState : ITestState
    {
        public ITimer Timer { get; }
        public IGlobalCounters Counters { get; }
        public IThreadPoolStats ThreadPool { get; }

        public TestState(ITimer timer, IGlobalCounters counters, IThreadPoolStats threadPool)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));
            if (counters == null)
                throw new ArgumentNullException(nameof(counters));
            if (threadPool == null)
                throw new ArgumentNullException(nameof(threadPool));

            Timer = timer;
            Counters = counters;
            ThreadPool = threadPool;
        }
    }
}