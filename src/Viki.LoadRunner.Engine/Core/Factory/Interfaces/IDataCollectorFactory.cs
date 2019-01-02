using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Factory.Interfaces
{
    public interface IDataCollectorFactory
    {
        IDataCollector Create(IIterationResult iterationContext);
    }
}