using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Factory.Interfaces
{
    public interface IReplayScenarioHandlerFactory
    {
        IReplayScenarioHandler Create(IIterationControl iterationContext);
    }
}