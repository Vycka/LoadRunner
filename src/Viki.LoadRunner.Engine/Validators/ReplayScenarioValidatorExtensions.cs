using System;
using Viki.LoadRunner.Engine.Core.Collector;
using Viki.LoadRunner.Engine.Core.Factory;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Pool;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer;
using Viki.LoadRunner.Engine.Strategies.Replay.Data;
using Viki.LoadRunner.Engine.Strategies.Replay.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Scenario;

namespace Viki.LoadRunner.Engine.Validators
{
    public static class ReplayScenarioValidatorExtensions
    {
        public static IterationResult Validate<TData>(this IReplayScenario<TData> scenario, DataItem data)
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

        public static IterationResult Validate<TData>(this IReplayScenario<TData> scenario, TData data)
        {
            DataItem dataItem = new DataItem(TimeSpan.Zero, data);

            return scenario.Validate(dataItem);
        }
    }
}