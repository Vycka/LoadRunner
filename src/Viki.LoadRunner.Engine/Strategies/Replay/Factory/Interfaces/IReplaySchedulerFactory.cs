using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Factory.Interfaces
{
    public interface IReplaySchedulerFactory
    {
        IScheduler Create(IReplayScenarioHandler scenarioHandler, int threadId);
    }
}