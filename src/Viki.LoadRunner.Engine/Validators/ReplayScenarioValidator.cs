using System;
using Viki.LoadRunner.Engine.Core.Collector;
using Viki.LoadRunner.Engine.Core.Factory;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Pool;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer;
using Viki.LoadRunner.Engine.Strategies.Replay.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Reader;
using Viki.LoadRunner.Engine.Strategies.Replay.Scenario;

namespace Viki.LoadRunner.Engine.Validators
{
    // TODO: Move these validators to engine project
    public class ReplayScenarioValidator<TData> : IValidator
    {
        private readonly IScenarioFactory _factory;
        private readonly DataItem _data;

        public ReplayScenarioValidator(IScenarioFactory factory, DataItem data)
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
            IReplayScenario<TData> scenario = (IReplayScenario<TData>)_factory.Create();

            return Validate(scenario, _data);
        }

        public static IterationResult Validate(IReplayScenario<TData> scenario, DataItem data)
        {
            ExecutionTimer timer = new ExecutionTimer();
            IUniqueIdFactory<int> idFactory = new IdFactory();
            IIterationControl context = new IterationContext(0, timer);

            ReplayScenarioHandler<TData> handler = new ReplayScenarioHandler<TData>(idFactory, scenario, context);

            timer.Start();

            handler.Init();
            handler.PrepareNext();
            handler.SetData(data.Value, data.TimeStamp);
            handler.Execute();

            IterationResult result = new IterationResult(context, new WorkerThreadStats());

            handler.Cleanup();

            timer.Stop();

            return result;
        }
    }
}