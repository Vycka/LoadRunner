using System;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Scenario;
using Viki.LoadRunner.Engine.Executor.Threads.Scheduler;
using Viki.LoadRunner.Engine.Executor.Threads.Stats;
using Viki.LoadRunner.Engine.Executor.Threads.Strategy;
using Viki.LoadRunner.Engine.Executor.Timer;
using Viki.LoadRunner.Engine.Strategies;

namespace Viki.LoadRunner.Engine.Executor.Threads.Factory
{
    public class ScenarioTaskFactory : IWorkerTaskFactory
    {
        private readonly Type _scenarioType;
        private readonly ITimer _timer;
        private readonly ISpeedStrategy _speedStrategy;
        private readonly IThreadPool _threadPool;
        private readonly IResultsAggregator _aggregator;

        private readonly IterationContextFactory _iterationContextFactory;
        private readonly IUniqueIdFactory<int> _globalIdFactory;

        public ScenarioTaskFactory(Type scenarioType, ITimer timer, ISpeedStrategy speedStrategy, IThreadPool threadPool, object initialUserData, IResultsAggregator aggregator)
        {
            if (scenarioType == null)
                throw new ArgumentNullException(nameof(scenarioType));
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));
            if (speedStrategy == null)
                throw new ArgumentNullException(nameof(speedStrategy));
            if (threadPool == null)
                throw new ArgumentNullException(nameof(threadPool));
            if (aggregator == null)
                throw new ArgumentNullException(nameof(aggregator));

            _scenarioType = scenarioType;
            _timer = timer;
            _speedStrategy = speedStrategy;
            _threadPool = threadPool;
            _aggregator = aggregator;

            _iterationContextFactory = new IterationContextFactory(_timer, initialUserData);
            _globalIdFactory = new IdFactory();
        }

        public IWorkerTask Create()
        {
            ILoadTestScenario scenarioInstance = (ILoadTestScenario)Activator.CreateInstance(_scenarioType);
            ITestContextControl testContext = _iterationContextFactory.Create();

            ScenarioHandlerEx scenarioHandler = new ScenarioHandlerEx(_globalIdFactory, scenarioInstance, testContext);

            SpeedStrategyHandler strategyHandler = new SpeedStrategyHandler(_speedStrategy, testContext, _threadPool);
            SchedulerEx scheduler = new SchedulerEx(strategyHandler, _threadPool, _timer);

            // TestContext still needs to have a split here, otherwise DataCollector looks like crap
            // smth like IIterationResult to keep modifieble state
            // Control part controls one part, and Context part controls other.
            // Or still monolith context with redesigned interfaces on it from scratch
            // SHould not compile atm

            DataCollector collector = new DataCollector(testContext, _aggregator, _threadPool);

            //  

            ScenarioWorkerTask scenarioWorkerTask = new ScenarioWorkerTask(scheduler, scenarioHandler, collector);
        }
    }

    public interface IWorkerTaskFactory
    {
        IWorkerTask Create();
    }
}