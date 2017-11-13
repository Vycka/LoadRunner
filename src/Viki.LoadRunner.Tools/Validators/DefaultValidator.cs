using System;
using Viki.LoadRunner.Engine.Core.Collector;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Tools.Validators
{
    public class DefaultValidator : IValidator
    {
        private readonly IScenarioFactory _factory;

        public DefaultValidator(IScenarioFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            _factory = factory;
        }

        public IterationResult Validate()
        {
            return ((IScenario)_factory.Create()).Validate();
        }
    }
}