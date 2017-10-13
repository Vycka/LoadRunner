using System;
using Viki.LoadRunner.Engine.Executor.Threads.Factory.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
using Viki.LoadRunner.Engine.Executor.Timer;
using Viki.LoadRunner.Engine.Framework.Interfaces;

namespace Viki.LoadRunner.Engine.Framework
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