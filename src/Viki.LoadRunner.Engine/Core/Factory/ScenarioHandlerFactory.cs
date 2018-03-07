using System;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;


namespace Viki.LoadRunner.Engine.Core.Factory
{
    public class ScenarioHandlerFactory : IScenarioHandlerFactory
    {
        private readonly IFactory<IScenario> _factory;
        private readonly IUniqueIdFactory<int> _globalIdFactory;

        public ScenarioHandlerFactory(IFactory<IScenario> factory, IUniqueIdFactory<int> globalIdFactory)
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
            IScenario scenarioInstance = _factory.Create();
            IScenarioHandler scenarioHandler = new ScenarioHandler(_globalIdFactory, scenarioInstance, iterationContext);

            return scenarioHandler;
        }
    }
}