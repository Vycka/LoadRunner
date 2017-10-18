using System;
using System.Threading;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Core.Counter;
using Viki.LoadRunner.Engine.Core.Counter.Interfaces;
using Viki.LoadRunner.Engine.Core.Factory;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.State;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;
using Viki.LoadRunner.Engine.Core.Worker;
using Viki.LoadRunner.Engine.Core.Worker.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Adapter.Aggregator;
using Viki.LoadRunner.Engine.Strategies.Custom.Adapter.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Factory;
using Viki.LoadRunner.Engine.Strategies.Custom.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Interfaces;
using ThreadPool = Viki.LoadRunner.Engine.Core.Pool.ThreadPool;

namespace Viki.LoadRunner.Engine.Strategies.Custom
{
    public class CustomStrategy : IStrategy
    {
        public ITimer Timer => _timer;
        private readonly ExecutionTimer _timer;

        protected readonly ICustomStrategySettings Settings;


        private IErrorHandler _errorHandler;
        private IUniqueIdFactory<int> _globalIdFactory;
        private IThreadPoolCounter _counter;

        private ThreadPool _pool;


        private ISpeedStrategy _speed;
        private IThreadingStrategy _threading;
        private ILimitStrategy _limit;
        private ITestState _state;
        private IResultsAggregator _aggregator;


        // Could ICustomStrategySettings
        public CustomStrategy(ICustomStrategySettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            Settings = settings;

            _timer = new ExecutionTimer();
        }

        public virtual void Start()
        {
            InitializeState();

            InitialThreadingSetup();

            _aggregator.Begin();

            _timer.Start(); // This line also releases Worker-Threads from wait in IPrewait
        }

        protected virtual void InitializeState()
        {
            _counter = new ThreadPoolCounter();
            _errorHandler = new ErrorHandler();
            _globalIdFactory = new IdFactory();

            _state = new TestState(_timer, _globalIdFactory, _counter);

            _speed = PriorityStrategyFactory.Create(Settings.Speeds, _timer);
            _limit = new LimitsHandler(Settings.Limits);
            _threading = Settings.Threading;
            _aggregator = new AsyncResultsAggregator(Settings.Aggregators);

            _pool = new ThreadPool(CreateWorkerThreadFactory(), _counter);
        }

        public virtual bool HeartBeat()
        {
            _errorHandler.Assert();

            _threading.HeartBeat(_pool, _state);
            _speed.HeartBeat(_state);

            return _limit.StopTest(_state);
        }

        public virtual void Stop()
        {
            _pool?.StopAndDispose((int)Settings.FinishTimeout.TotalMilliseconds);
            _pool = null;

            _timer.Stop();
            _aggregator.End();

            _errorHandler.Assert();
        }

        protected virtual IWorkerThreadFactory CreateWorkerThreadFactory()
        {
            IIterationContextFactory iterationContextFactory = CreateIterationContextFactory();
            IScenarioHandlerFactory scenarioHandlerFactory = CreateScenarioHandlerFactory();
            ISchedulerFactory schedulerFactory = CreateSchedulerFactory();
            IDataCollectorFactory dataCollectorFactory = CreateDataCollectorFactory();

            IPrewait prewait = new TimerBasedPrewait(_timer);

            IWorkerThreadFactory threadFactory = new ScenarioThreadFactory(
                iterationContextFactory,
                scenarioHandlerFactory,
                schedulerFactory,
                dataCollectorFactory,
                prewait,
                _errorHandler
            );

            return threadFactory;
        }

        protected virtual IDataCollectorFactory CreateDataCollectorFactory()
        {
            return new DataCollectorFactory(_aggregator, _counter);
        }

        protected virtual ISchedulerFactory CreateSchedulerFactory()
        {
            return new SchedulerFactory(_timer, _speed, _counter);
        }

        protected virtual IScenarioHandlerFactory CreateScenarioHandlerFactory()
        {
            return new ScenarioHandlerFactory(Settings.TestScenarioType, _globalIdFactory);
        }

        protected virtual IIterationContextFactory CreateIterationContextFactory()
        {
            return new IterationContextFactory(_timer, Settings.InitialUserData);
        }

        private void InitialThreadingSetup()
        {
            _threading.Setup(_pool);

            while (_counter.CreatedThreadCount != _counter.InitializedThreadCount)
            {
                Thread.Sleep(100);
                _errorHandler.Assert();
            }
        }
    }
}