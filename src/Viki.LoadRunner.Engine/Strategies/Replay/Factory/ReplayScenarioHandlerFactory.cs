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
        private readonly Type _scenarioType;
        private readonly IUniqueIdFactory<int> _globalIdFactory;

        public ReplayScenarioHandlerFactory(Type scenarioType, IUniqueIdFactory<int> globalIdFactory)
        {
            if (scenarioType == null)
                throw new ArgumentNullException(nameof(scenarioType));
            if (globalIdFactory == null)
                throw new ArgumentNullException(nameof(globalIdFactory));

            _scenarioType = scenarioType;
            _globalIdFactory = globalIdFactory;
        }

        public IReplayScenarioHandler Create(IIterationControl iterationContext)
        {
            IReplayScenario<TData> scenarioInstance = (IReplayScenario<TData>)Activator.CreateInstance(_scenarioType);
            IReplayScenarioHandler scenarioHandler = new ReplayScenarioHandler<TData>(_globalIdFactory, scenarioInstance, iterationContext);

            return scenarioHandler;
        }
    }
}