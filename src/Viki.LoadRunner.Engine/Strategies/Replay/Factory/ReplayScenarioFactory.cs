using System;
using Viki.LoadRunner.Engine.Core.Factory;
using Viki.LoadRunner.Engine.Strategies.Replay.Factory.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Factory
{
    public class ReplayScenarioFactory<TData> : ReflectionFactory<IReplayScenario<TData>>, IReplayScenarioFactory<TData>
    {
        public ReplayScenarioFactory(Type replayScenarioType) : base(replayScenarioType)
        {
        }
    }
}