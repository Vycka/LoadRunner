using System;
using Viki.LoadRunner.Engine.Core.Collector;
using Viki.LoadRunner.Engine.Strategies.Replay.Data;
using Viki.LoadRunner.Engine.Strategies.Replay.Factory.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Interfaces;

namespace Viki.LoadRunner.Engine.Validators
{
    // TODO: Move these validators to engine project
    public class ReplayScenarioValidator<TData> : IValidator
    {
        private readonly IReplayScenarioFactory<TData> _factory;
        private readonly DataItem _data;

        public ReplayScenarioValidator(IReplayScenarioFactory<TData> factory, DataItem data)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _factory = factory;
            _data = data;
        }
        
        public IterationResult Validate()
        {
            IReplayScenario<TData> scenario = _factory.Create();

            return scenario.Validate(_data);
        }
    }
}