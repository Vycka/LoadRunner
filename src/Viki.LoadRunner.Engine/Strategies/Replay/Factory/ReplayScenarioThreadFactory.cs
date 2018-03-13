using System;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Pool.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.Worker.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Factory.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Factory
{
    public class ReplayScenarioThreadFactory : IThreadFactory
    {
        private readonly IReplayScenarioHandlerFactory _scenarioHandlerFactory;
        private readonly IReplaySchedulerFactory _schedulerFactory;
        private readonly IDataCollectorFactory _dataCollectorFactory;
        private readonly IScenarioThreadFactory _threadFactory;
        private readonly IIterationContextFactory _iterationContextFactory;

        public ReplayScenarioThreadFactory(IIterationContextFactory iterationContextFactory, IReplayScenarioHandlerFactory scenarioHandlerFactory, IReplaySchedulerFactory schedulerFactory, IDataCollectorFactory dataCollectorFactory, IScenarioThreadFactory threadFactory)
        {
            if (iterationContextFactory == null)
                throw new ArgumentNullException(nameof(iterationContextFactory));
            if (scenarioHandlerFactory == null)
                throw new ArgumentNullException(nameof(scenarioHandlerFactory));
            if (schedulerFactory == null)
                throw new ArgumentNullException(nameof(schedulerFactory));
            if (dataCollectorFactory == null)
                throw new ArgumentNullException(nameof(dataCollectorFactory));
            if (threadFactory == null)
                throw new ArgumentNullException(nameof(threadFactory));


            _iterationContextFactory = iterationContextFactory;
            _scenarioHandlerFactory = scenarioHandlerFactory;
            _schedulerFactory = schedulerFactory;
            _dataCollectorFactory = dataCollectorFactory;
            _threadFactory = threadFactory;
        }

        public IThread Create()
        {
            IIterationControl iterationContext = _iterationContextFactory.Create();

            IReplayScenarioHandler handler = _scenarioHandlerFactory.Create(iterationContext);
            IScheduler scheduler = _schedulerFactory.Create(handler, iterationContext.ThreadId);
            IDataCollector collector = _dataCollectorFactory.Create(iterationContext);

            IThread thread = _threadFactory.Create(scheduler, handler, collector);

            return thread;
        }
    }
}