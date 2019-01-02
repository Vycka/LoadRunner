using System;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Factory
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

        public IIterationControl Create()
        {
            int newThreadId = _threadIdFactory.Next();

            return new IterationContext(newThreadId, _timer, _initialUserData);
        }
    }
}