using System;
using System.Threading;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Executor.Counter;
using Viki.LoadRunner.Engine.Executor.Counter.Interfaces;
using Viki.LoadRunner.Engine.Executor.Factory;
using Viki.LoadRunner.Engine.Executor.Factory.Interfaces;
using Viki.LoadRunner.Engine.Executor.State;
using Viki.LoadRunner.Engine.Executor.State.Interfaces;
using Viki.LoadRunner.Engine.Executor.Timer;
using Viki.LoadRunner.Engine.Executor.Timer.Interfaces;
using Viki.LoadRunner.Engine.Executor.Worker;
using Viki.LoadRunner.Engine.Executor.Worker.Interfaces;
using Viki.LoadRunner.Engine.Presets.Adapter.Aggregator;
using Viki.LoadRunner.Engine.Presets.Adapter.Limit;
using Viki.LoadRunner.Engine.Presets.Factory;
using Viki.LoadRunner.Engine.Presets.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Interfaces;
using ThreadPool = Viki.LoadRunner.Engine.Executor.Pool.ThreadPool;

namespace Viki.LoadRunner.Engine.Presets
{
    public class CustomStrategy : IStrategy
    {
        public ITimer Timer => _timer;
        private readonly ExecutionTimer _timer;

        private readonly ICustomStrategySettings _settings;

        private IErrorHandler _errorHandler;
        private IUniqueIdFactory<int> _globalIdFactory;
        

        private ThreadPool _pool;
        private IThreadPoolCounter _counter;
        
        private ISpeedStrategy _speed;
        private IThreadingStrategy _threading;
        private ILimitStrategy _limit;
        private ITestState _state;
        private IResultsAggregator _aggregator;

        public CustomStrategy(ICustomStrategySettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _settings = settings;

            _timer = new ExecutionTimer();
        }

        public virtual void Start()
        {
            _counter = new ThreadPoolCounter();
            
            _errorHandler = new ErrorHandler();
            _globalIdFactory = new IdFactory();

            _speed = PriorityStrategyFactory.Create(_settings.Speeds, _timer);
            _limit = new LimitsHandler(_settings.Limits);
            _threading = _settings.Threading;
            _state = new TestState(_timer, _globalIdFactory, _counter);
            _aggregator = new AsyncResultsAggregator(_settings.Aggregators);

            _pool = new ThreadPool(CreateWorkerThreadFactory(), _counter);

            InitialThreadingSetup();

            _aggregator.Begin();

            _timer.Start(); // This line also releases Worker-Threads from wait in IPrewait
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
            _pool?.StopAndDispose((int)_settings.FinishTimeout.TotalMilliseconds);
            _pool = null;

            _timer.Stop();
            _aggregator.End();

            _errorHandler.Assert();
        }

        private IWorkerThreadFactory CreateWorkerThreadFactory()
        {
            IWorkerThreadFactory threadFactory = new ScenarioThreadFactory(
                    _settings.TestScenarioType,
                    _timer,
                    _speed,
                    _counter,
                    _settings.InitialUserData,
                    _aggregator,
                    _globalIdFactory,
                    _errorHandler
                );

            return threadFactory;
        }

        private void InitialThreadingSetup()
        {
            _settings.Threading.Setup(_pool);

            while (_counter.CreatedThreadCount != _counter.InitializedThreadCount)
            {
                Thread.Sleep(100);
                _errorHandler.Assert();
            }
        }
    }
}