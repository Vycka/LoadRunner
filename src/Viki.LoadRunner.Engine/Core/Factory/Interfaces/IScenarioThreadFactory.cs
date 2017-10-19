using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.Worker.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Factory.Interfaces
{
    public interface IScenarioThreadFactory
    {
        IThread Create(IScheduler scheduler, IScenarioHandler handler, IDataCollector collector);
    }
}