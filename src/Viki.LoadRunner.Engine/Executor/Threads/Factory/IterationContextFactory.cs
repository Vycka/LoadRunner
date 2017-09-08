using System;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
using Viki.LoadRunner.Engine.Executor.Timer;

namespace Viki.LoadRunner.Engine.Executor.Threads.Factory
{
    public interface IIterationContextFactory
    {
        ITestContextControl Create();
    }

    public class IterationContextFactory : IIterationContextFactory
    {
        private readonly ITimer _timer;
        private readonly object _initialUserData;

        private readonly IUniqueIdFactory<int> _threadIdFactory;

        public IterationContextFactory(ITimer timer, object initialUserData)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));
            if (initialUserData == null)
                throw new ArgumentNullException(nameof(initialUserData));
            _timer = timer;
            _initialUserData = initialUserData;

            _threadIdFactory = new IdFactory();
        }

        public ITestContextControl Create()
        {
            int newThreadId = _threadIdFactory.Next();

            return new TestContext(newThreadId, _timer, _initialUserData);
        }
    }
}