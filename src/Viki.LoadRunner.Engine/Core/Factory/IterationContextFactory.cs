using System;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Generator;
using Viki.LoadRunner.Engine.Core.Generator.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Factory
{
    public class IterationContextFactory : IIterationContextFactory
    {
        private readonly ITimer _timer;
        private readonly object _initialUserData;

        private readonly IUniqueIdGenerator<int> _threadIdGenerator;

        public IterationContextFactory(ITimer timer, object initialUserData)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));

            _timer = timer;
            _initialUserData = initialUserData;

            _threadIdGenerator = new ThreadSafeIdGenerator();
        }

        public IIterationControl Create()
        {
            int newThreadId = _threadIdGenerator.Next();

            return new IterationContext(newThreadId, _timer, _initialUserData);
        }
    }
}