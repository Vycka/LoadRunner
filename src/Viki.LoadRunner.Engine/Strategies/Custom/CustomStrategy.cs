using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Collector;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline.Interfaces;
using Viki.LoadRunner.Engine.Core.Counter;
using Viki.LoadRunner.Engine.Core.Counter.Interfaces;
using Viki.LoadRunner.Engine.Core.Factory;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Pool.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.State;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer;
using Viki.LoadRunner.Engine.Core.Worker;
using Viki.LoadRunner.Engine.Core.Worker.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Adapter.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Factory;
using Viki.LoadRunner.Engine.Strategies.Custom.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Interfaces;
using Viki.LoadRunner.Engine.Utils;
using ThreadPool = Viki.LoadRunner.Engine.Core.Pool.ThreadPool;

namespace Viki.LoadRunner.Engine.Strategies.Custom
{
    // TODO: Split into Custom Stragegy : Generic Strategy
    // - Generic one will have n handlers like Speed / Collector / etc...
    //   Using dynamic compiling?
    // - Generic could be extended for ReplayStrategy, CustomStrategy (though not sure how to make them aggregatable)
    // - Not sure how with limits
    public class CustomStrategy : IStrategy
    {
        private readonly ExecutionTimer _timer;

        private readonly ICustomStrategySettings _settings;

        private IErrorHandler _errorHandler;

        private ThreadPool _pool;
        private IThreadPoolCounter _threadPoolCounter;

        private GlobalCounters _globalCounters;

        private ISpeedStrategy _speed;
        private IThreadingStrategy _threading;
        private LimitsHandler _limit;
        private ITestState _state;
        private PipelineDataAggregator _aggregator;

        public CustomStrategy(ICustomStrategySettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _settings = (ICustomStrategySettings) settings.ShallowCopy();

            _timer = new ExecutionTimer();
        }

        public ITestState Start()
        {
            _threadPoolCounter = new ThreadPoolCounter();

            PipeFactory<IResult> pipeFactory = new PipeFactory<IResult>();
            _aggregator = new PipelineDataAggregator(_settings.Aggregators, pipeFactory);
            _aggregator.Start();

            _globalCounters = GlobalCounters.CreateDefault();

            _errorHandler = new ErrorHandler();

            _limit = new LimitsHandler(_settings.Limits);
            _threading = _settings.Threading;

            _state = new TestState(_timer, _globalCounters, _threadPoolCounter);

            _speed = PriorityStrategyFactory.Create(_settings.Speeds, _timer);
            _speed.Setup(_state);

            IIterationContextFactory iterationContextFactory = CreateIterationContextFactory();
            IScenarioHandlerFactory scenarioHandlerFactory = CreateScenarioHandlerFactory();
            ISchedulerFactory schedulerFactory = CreateSchedulerFactory();
            IDataCollectorFactory dataCollectorFactory = CreateDataCollectorFactory(pipeFactory, _threadPoolCounter);
            IScenarioThreadFactory scenarioThreadFactory = CreateScenarioThreadFactory();

            IThreadFactory threadFactory = new ScenarioThreadFactory(
                iterationContextFactory,
                scenarioHandlerFactory,
                schedulerFactory,
                dataCollectorFactory,
                scenarioThreadFactory
            );

            _pool = new ThreadPool(threadFactory, _threadPoolCounter);

            InitialThreadingSetup();

            _timer.Start(); // This line also releases Worker-Threads from wait in IPrewait

            return _state;
        }

        public bool HeartBeat()
        {
            if (_aggregator.Error != null)
                throw _aggregator.Error;

            _errorHandler.Assert();

            _threading.HeartBeat(_pool, _state);
            _speed.HeartBeat(_state);

            return _limit.StopTest(_state);
        }

        public void Stop()
        {
            _pool?.StopAndDispose((int) _settings.FinishTimeout.TotalMilliseconds);
            _pool = null;

            _timer.Stop();
            _aggregator.End();

            _errorHandler.Assert();
        }

        private IScenarioThreadFactory CreateScenarioThreadFactory()
        {
            IPrewait prewait = new TimerBasedPrewait(_timer);
            IScenarioThreadFactory factory = new ThreadFactory(prewait, _errorHandler);

            return factory;
        }

        private IDataCollectorFactory CreateDataCollectorFactory(IPipeFactory<IResult> pipeFactory, IThreadPoolStats poolStats)
        {
            IDataCollectorFactory result;
            if (_settings.Aggregators.IsNullOrEmpty())
                result = new NullDataCollectorFactory();
            else
                result = new PipeDataCollectorFactory(pipeFactory, poolStats);

            return result;
        }

        private ISchedulerFactory CreateSchedulerFactory()
        {
            ISchedulerFactory result;

            if (_settings.Speeds.IsNullOrEmpty())
                result = new NullSchedulerFactory();
            else
                result = new SchedulerFactory(_timer, _speed, _threadPoolCounter);

            return result;
        }

        private IScenarioHandlerFactory CreateScenarioHandlerFactory()
        {
            return new ScenarioHandlerFactory(_settings.ScenarioFactory, _globalCounters);
        }

        private IIterationContextFactory CreateIterationContextFactory()
        {
            return new IterationContextFactory(_timer, _settings.InitialUserData);
        }

        private void InitialThreadingSetup()
        {
            _threading.Setup(_pool);

            while (_threadPoolCounter.CreatedThreadCount != _threadPoolCounter.InitializedThreadCount)
            {
                Thread.Sleep(100);
                _errorHandler.Assert();
            }
        }
    }
}