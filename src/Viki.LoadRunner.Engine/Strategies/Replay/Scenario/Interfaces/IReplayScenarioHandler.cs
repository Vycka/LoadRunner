using Viki.LoadRunner.Engine.Core.Worker.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Scenario.Interfaces
{
    public interface IReplayScenarioHandler : IScenarioHandler
    {
        void SetData(object data);
    }
}