using System;
using Viki.LoadRunner.Engine.Core.Worker.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Scheduler.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Scenario.Interfaces
{
    public interface IReplayScenarioHandler : IScenarioHandler
    {
        bool SetData(object data, TimeSpan target);
    }
}