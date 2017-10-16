using System;
using System.Threading;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.Counters;
using Viki.LoadRunner.Engine.Executor.Strategy.Counters.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.Factory;
using Viki.LoadRunner.Engine.Executor.Strategy.Factory.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.State;
using Viki.LoadRunner.Engine.Executor.Strategy.State.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.Timer;
using Viki.LoadRunner.Engine.Executor.Strategy.Timer.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.Workers;
using Viki.LoadRunner.Engine.Executor.Strategy.Workers.Interfaces;
using Viki.LoadRunner.Engine.Presets.Adapters.Aggregator;
using Viki.LoadRunner.Engine.Presets.Adapters.Limits;
using Viki.LoadRunner.Engine.Presets.Factories;
using Viki.LoadRunner.Engine.Presets.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Limit;
using ThreadPool = Viki.LoadRunner.Engine.Executor.Strategy.Pool.ThreadPool;

namespace Viki.LoadRunner.Engine.Presets
{
    // TODO: Derrive this class and one could can have UI.
    public class CustomStrategy : IStrategy
    {
        private readonly ICustomStrategySettings _settings;

        private IErrorHandler _errorHandler;
        private IUniqueIdFactory<int> _globalIdFactory;
        private ITimerControl _timer;

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
        }

        void IStrategy.Start()
        {
            _counter = new ThreadPoolCounter();
            
            _errorHandler = new ErrorHandler();
            _globalIdFactory = new IdFactory();
            _timer = new ExecutionTimer();

            _speed = PriorityStrategyFactory.Create(_settings.Speed, _timer);
            _limit = new LimitsHandler(_settings.Limits);
            _threading = _settings.Threading;
            _state = new TestState(_timer, _globalIdFactory, _counter);
            _aggregator = new AsyncResultsAggregator(_settings.Aggregators);

            _pool = new ThreadPool(CreateWorkerThreadFactory(), _counter);

            InitialThreadingSetup();

            _aggregator.Begin();

            _timer.Start(); // This line also releases Worker-Threads from wait in IPrewait
        }

        bool IStrategy.HeartBeat()
        {
            _errorHandler.Assert();

            _threading.HeartBeat(_pool, _state);
            _speed.HeartBeat(_state);

            return _limit.StopTest(_state);
        }

        void IStrategy.Stop()
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