using System;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Factory.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Scenario;
using Viki.LoadRunner.Engine.Strategies.Replay.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Factory
{
    public class ReplayScenarioHandlerFactory<TData> : IReplayScenarioHandlerFactory
    {
        private readonly IGlobalCountersControl _globalCounters;
        private readonly IFactory<IReplayScenario<TData>> _scenarioFactory;


        public ReplayScenarioHandlerFactory(IFactory<IReplayScenario<TData>> scenarioFactory, IGlobalCountersControl globalCounters)
        {
            if (scenarioFactory == null)
                throw new ArgumentNullException(nameof(scenarioFactory));
            if (globalCounters == null)
                throw new ArgumentNullException(nameof(globalCounters));

            _scenarioFactory = scenarioFactory;
            _globalCounters = globalCounters;
        }

        public IReplayScenarioHandler Create(IIterationControl iterationContext)
        {
            IReplayScenario<TData> scenarioInstance = _scenarioFactory.Create(iterationContext.ThreadId);
            IReplayScenarioHandler scenarioHandler = new ReplayScenarioHandler<TData>(_globalCounters, scenarioInstance, iterationContext);

            return scenarioHandler;
        }
    }
}