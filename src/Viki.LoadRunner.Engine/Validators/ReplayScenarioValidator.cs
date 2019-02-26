using System;
using Viki.LoadRunner.Engine.Core.Collector;
using Viki.LoadRunner.Engine.Strategies.Replay.Data;
using Viki.LoadRunner.Engine.Strategies.Replay.Factory.Interfaces;

namespace Viki.LoadRunner.Engine.Validators
{
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
        
        public IterationResult Validate(int threadId = 0, int threadIterationId = 0, int globalIterationId = 0)
        {
            return _factory.Create(threadId).Validate(_data, threadId, threadIterationId, globalIterationId);
        }
    }
}