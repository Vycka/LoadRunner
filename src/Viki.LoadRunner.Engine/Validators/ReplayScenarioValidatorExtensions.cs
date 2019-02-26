using System;
using Viki.LoadRunner.Engine.Core.Collector;
using Viki.LoadRunner.Engine.Core.Counter;
using Viki.LoadRunner.Engine.Core.Factory;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Generator;
using Viki.LoadRunner.Engine.Core.Generator.Interfaces;
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
        public static IterationResult Validate<TData>(this IReplayScenario<TData> scenario, DataItem data, int threadId = 0, int threadIterationId = 0, int globalIterationId = 0)
        {
            ExecutionTimer timer = new ExecutionTimer();
            ThreadSafeCounter errorsCounter = new ThreadSafeCounter();
            GlobalCounters globalCounters = new GlobalCounters(errorsCounter, new MockedIdGenerator(globalIterationId));
            IIterationControl context = new IterationContext(threadId, timer);

            ReplayScenarioHandler<TData> handler = new ReplayScenarioHandler<TData>(globalCounters, new MockedIdGenerator(threadIterationId), scenario, context);

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

        public static IterationResult Validate<TData>(this IReplayScenario<TData> scenario, TData data, int threadId = 0, int threadIterationId = 0, int globalIterationId = 0)
        {
            DataItem dataItem = new DataItem(TimeSpan.Zero, data);

            return scenario.Validate(dataItem, threadId, threadIterationId, globalIterationId);
        }
    }
}