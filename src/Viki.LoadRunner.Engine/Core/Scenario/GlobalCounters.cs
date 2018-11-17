using System;
using Viki.LoadRunner.Engine.Core.Counter;
using Viki.LoadRunner.Engine.Core.Counter.Interfaces;
using Viki.LoadRunner.Engine.Core.Factory;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scenario
{
    public class GlobalCounters : IGlobalCountersControl, IGlobalCounters
    {
        public IUniqueIdFactory<int> IterationId { get; }
        public ICounter Errors { get; }

        public int ErrorCount => Errors.Value;
        public int LastGlobalIterationId => IterationId.Current;

        public static GlobalCounters CreateDefault()
        {
            return new GlobalCounters(new ThreadSafeCounter(), new IdFactory());
        }

        public GlobalCounters(ICounter errorsCounter, IUniqueIdFactory<int> iterationIdCounter)
        {
            Errors = errorsCounter ?? throw new ArgumentNullException(nameof(errorsCounter));
            IterationId = iterationIdCounter ?? throw new ArgumentNullException(nameof(iterationIdCounter));
        }
    }
}