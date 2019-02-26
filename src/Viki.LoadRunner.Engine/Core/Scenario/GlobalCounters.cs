using System;
using Viki.LoadRunner.Engine.Core.Counter;
using Viki.LoadRunner.Engine.Core.Counter.Interfaces;
using Viki.LoadRunner.Engine.Core.Generator;
using Viki.LoadRunner.Engine.Core.Generator.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scenario
{
    public class GlobalCounters : IGlobalCountersControl, IGlobalCounters
    {
        public IUniqueIdGenerator<int> IterationId { get; }
        public ICounter Errors { get; }

        public int ErrorCount => Errors.Value;
        public int LastGlobalIterationId => IterationId.Current;

        public static GlobalCounters CreateDefault()
        {
            return new GlobalCounters(new ThreadSafeCounter(), new ThreadSafeIdGenerator());
        }

        public GlobalCounters(ICounter errorsCounter, IUniqueIdGenerator<int> iterationIdCounter)
        {
            Errors = errorsCounter ?? throw new ArgumentNullException(nameof(errorsCounter));
            IterationId = iterationIdCounter ?? throw new ArgumentNullException(nameof(iterationIdCounter));
        }
    }
}