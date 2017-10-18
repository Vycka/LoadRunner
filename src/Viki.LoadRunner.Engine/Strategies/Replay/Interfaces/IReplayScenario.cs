using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Interfaces
{
    public interface IReplayScenario<TData> : IScenario
    {
        void SetData(TData data);
    }
}