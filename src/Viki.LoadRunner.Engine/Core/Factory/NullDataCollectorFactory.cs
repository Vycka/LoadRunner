using Viki.LoadRunner.Engine.Core.Collector;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Factory
{
    public class NullDataCollectorFactory : IDataCollectorFactory
    {
        public IDataCollector Create(IIterationResult iterationContext)
        {
            return new NullDataCollector();
        }
    }
}