using System;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Pool.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.Worker.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Factory
{
    public class ScenarioThreadFactory : IThreadFactory
    {
        private readonly IScenarioHandlerFactory _scenarioHandlerFactory;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IDataCollectorFactory _dataCollectorFactory;
        private readonly IScenarioThreadFactory _threadFactory;
        private readonly IIterationContextFactory _iterationContextFactory;

        public ScenarioThreadFactory(IIterationContextFactory iterationContextFactory, IScenarioHandlerFactory scenarioHandlerFactory, ISchedulerFactory schedulerFactory, IDataCollectorFactory dataCollectorFactory, IScenarioThreadFactory threadFactory)
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

            // IScenario handler
            IScenarioHandler scenarioHandler = _scenarioHandlerFactory.Create(iterationContext);
            // Scheduler which will Sleep() thread if it needs to wait due to speed limiting strategy. 
            IScheduler scheduler = _schedulerFactory.Create(iterationContext);
            // Data collector for results aggregation
            IDataCollector collector = _dataCollectorFactory.Create(iterationContext);

            IThread thread = _threadFactory.Create(scheduler, scenarioHandler, collector);

            return thread;
        }
    }
}