using System;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Scenario.Interfaces
{
    public interface IReplayScenarioHandler : IScenarioHandler
    {
        bool SetData(object data, TimeSpan target);
    }
}