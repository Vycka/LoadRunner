using System;
using Viki.LoadRunner.Engine.Executor.Strategy.Factory.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.State.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Strategy.State
{
    public class TestState : ITestState
    {
        private readonly ITimer _timer;
        private readonly IUniqueIdFactory<int> _globalId;
        private readonly IThreadPoolStats _threadPool;

        public TestState(ITimer timer, IUniqueIdFactory<int> globalId, IThreadPoolStats threadPool)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));
            if (globalId == null)
                throw new ArgumentNullException(nameof(globalId));
            if (threadPool == null)
                throw new ArgumentNullException(nameof(threadPool));

            _timer = timer;
            _globalId = globalId;
            _threadPool = threadPool;
        }

        public ITimer Timer => _timer;
        public int GlobalIterationId => _globalId.Current;
        public IThreadPoolStats ThreadPool => _threadPool;
    }
}