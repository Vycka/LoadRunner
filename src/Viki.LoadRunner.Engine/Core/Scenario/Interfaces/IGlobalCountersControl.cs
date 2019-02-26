using Viki.LoadRunner.Engine.Core.Counter.Interfaces;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Generator.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scenario.Interfaces
{
    public interface IGlobalCountersControl 
    {
        IUniqueIdGenerator<int> IterationId { get; }
        ICounter Errors { get; }
    }
}