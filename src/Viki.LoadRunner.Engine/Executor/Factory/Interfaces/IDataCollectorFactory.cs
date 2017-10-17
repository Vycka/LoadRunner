using Viki.LoadRunner.Engine.Executor.Collector.Interfaces;
using Viki.LoadRunner.Engine.Executor.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Factory.Interfaces
{
    public interface IDataCollectorFactory
    {
        IDataCollector Create(IIterationResult iterationContext);
    }
}