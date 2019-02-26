using System;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Generator;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;


namespace Viki.LoadRunner.Engine.Core.Factory
{
    public class ScenarioHandlerFactory : IScenarioHandlerFactory
    {
        private readonly IFactory<IScenario> _factory;
        private readonly IGlobalCountersControl _globalCounters;

        public ScenarioHandlerFactory(IFactory<IScenario> factory, IGlobalCountersControl globalCounters)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            if (globalCounters == null)
                throw new ArgumentNullException(nameof(globalCounters));

            _factory = factory;
            _globalCounters = globalCounters;
        }

        public IScenarioHandler Create(IIterationControl iterationContext)
        {
            IScenario scenarioInstance = _factory.Create(iterationContext.ThreadId);
            IScenarioHandler scenarioHandler = new ScenarioHandler(_globalCounters, new NotThreadSafeIdGenerator(), scenarioInstance, iterationContext);

            return scenarioHandler;
        }
    }
}