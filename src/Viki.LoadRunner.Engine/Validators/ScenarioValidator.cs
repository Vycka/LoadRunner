using System;
using Viki.LoadRunner.Engine.Core.Collector;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;

namespace Viki.LoadRunner.Engine.Validators
{
    public class ScenarioValidator : IValidator
    {
        private readonly IScenarioFactory _factory;

        public ScenarioValidator(IScenarioFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            _factory = factory;
        }

        public IterationResult Validate()
        {
            // TODO: Move all logic to here
            return _factory.Create().Validate();
        }
    }
}