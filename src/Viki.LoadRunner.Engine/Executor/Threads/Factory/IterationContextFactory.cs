using System;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Context.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Factory.Interfaces;
using Viki.LoadRunner.Engine.Executor.Timer;

namespace Viki.LoadRunner.Engine.Executor.Threads.Factory
{
    public class IterationContextFactory : IIterationContextFactory
    {
        private readonly ITimer _timer;
        private readonly object _initialUserData;

        private readonly IUniqueIdFactory<int> _threadIdFactory;

        public IterationContextFactory(ITimer timer, object initialUserData)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));

            _timer = timer;
            _initialUserData = initialUserData;

            _threadIdFactory = new IdFactory();
        }

        public IIterationContextControl Create()
        {
            int newThreadId = _threadIdFactory.Next();

            return new IterationContext(newThreadId, _timer, _initialUserData);
        }
    }
}