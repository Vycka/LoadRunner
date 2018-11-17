using Viki.LoadRunner.Engine.Core.Counter.Interfaces;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scenario.Interfaces
{
    public interface IGlobalCountersControl 
    {
        IUniqueIdFactory<int> IterationId { get; }
        ICounter Errors { get; }
    }
}