using System;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Worker;
using Viki.LoadRunner.Engine.Core.Worker.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Factory
{
    public class ScenarioHandlerFactory : IScenarioHandlerFactory
    {
        private readonly IScenarioFactory _factory;
        private readonly IUniqueIdFactory<int> _globalIdFactory;

        public ScenarioHandlerFactory(IScenarioFactory factory, IUniqueIdFactory<int> globalIdFactory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            if (globalIdFactory == null)
                throw new ArgumentNullException(nameof(globalIdFactory));

            _factory = factory;
            _globalIdFactory = globalIdFactory;
        }

        public IScenarioHandler Create(IIterationControl iterationContext)
        {
            IScenario scenarioInstance = (IScenario)_factory.Create();
            IScenarioHandler scenarioHandler = new ScenarioHandler(_globalIdFactory, scenarioInstance, iterationContext);

            return scenarioHandler;
        }
    }
}