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
        private readonly IScenarioFactory _scenarioFactory;
        private readonly IUniqueIdFactory<int> _globalIdFactory;

        public ReplayScenarioHandlerFactory(IScenarioFactory scenarioFactory, IUniqueIdFactory<int> globalIdFactory)
        {
            if (scenarioFactory == null)
                throw new ArgumentNullException(nameof(scenarioFactory));
            if (globalIdFactory == null)
                throw new ArgumentNullException(nameof(globalIdFactory));

            _scenarioFactory = scenarioFactory;
            _globalIdFactory = globalIdFactory;
        }

        public IReplayScenarioHandler Create(IIterationControl iterationContext)
        {
            IReplayScenario<TData> scenarioInstance = (IReplayScenario<TData>)_scenarioFactory.Create();
            IReplayScenarioHandler scenarioHandler = new ReplayScenarioHandler<TData>(_globalIdFactory, scenarioInstance, iterationContext);

            return scenarioHandler;
        }
    }
}