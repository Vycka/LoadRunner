using System;
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
using Viki.LoadRunner.Engine.Core.Timer;
using Viki.LoadRunner.Engine.Core.Worker;
using Viki.LoadRunner.Engine.Core.Worker.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Data.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Factory;
using Viki.LoadRunner.Engine.Strategies.Replay.Factory.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Interfaces;
using Viki.LoadRunner.Engine.Utils;
using ThreadPool = Viki.LoadRunner.Engine.Core.Pool.ThreadPool;

namespace Viki.LoadRunner.Engine.Strategies.Replay
{
    public class ReplayStrategy<TData> : IStrategy
    {
        private readonly IReplayStrategySettings<TData> _settings;
        private readonly ExecutionTimer _timer;

        private PipelineDataAggregator _aggregator;
        private IReplayDataReader _dataReader;
        private IErrorHandler _errorHandler;
        private IThreadPoolCounter _counter;
        private ThreadPool _pool;

        public ReplayStrategy(IReplayStrategySettings<TData> settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _settings = (IReplayStrategySettings<TData>)settings.ShallowCopy();
            _timer = new ExecutionTimer();
        }

        public virtual void Start()
        {
            _counter = new ThreadPoolCounter();
            PipeFactory<IResult> pipeFactory = new PipeFactory<IResult>();
            _aggregator = new PipelineDataAggregator(_settings.Aggregators, pipeFactory);
            _aggregator.Start();

            _errorHandler = new ErrorHandler();
            _dataReader = _settings.DataReader;
            _dataReader.Begin();

            IIterationContextFactory iterationContextFactory = CreateIterationContextFactory();
            IReplayScenarioHandlerFactory scenarioHandlerFactory = CreateScenarioHandlerFactory();
            IReplaySchedulerFactory schedulerFactory = CreateSchedulerFactory();
            IDataCollectorFactory dataCollectorFactory = CreateDataCollectorFactory(pipeFactory, _counter);
            IScenarioThreadFactory scenarioThreadFactory = CreateScenarioThreadFactory();

            IThreadFactory threadFactory = new ReplayScenarioThreadFactory(
                iterationContextFactory,
                scenarioHandlerFactory,
                schedulerFactory,
                dataCollectorFactory,
                scenarioThreadFactory
            );

            _pool = new ThreadPool(threadFactory, _counter);
            _pool.StartWorkersAsync(_settings.ThreadCount);

            while (_counter.CreatedThreadCount != _counter.InitializedThreadCount)
            {
                Thread.Sleep(100);
                _errorHandler.Assert();
            }

            _timer.Start(); // This line also releases Worker-Threads from wait in IPrewait
        }

        private IScenarioThreadFactory CreateScenarioThreadFactory()
        {
            IPrewait prewait = new TimerBasedPrewait(_timer);
            IScenarioThreadFactory factory = new ThreadFactory(prewait, _errorHandler);

            return factory;
        }

        private IIterationContextFactory CreateIterationContextFactory()
        {
            return new IterationContextFactory(_timer, _settings.InitialUserData);
        }

        private IReplayScenarioHandlerFactory CreateScenarioHandlerFactory()
        {
            return new ReplayScenarioHandlerFactory<TData>(_settings.ScenarioFactory, GlobalCounters.CreateDefault());
        }

        private IReplaySchedulerFactory CreateSchedulerFactory()
        {
            return new ReplaySchedulerFactory(_timer,  _dataReader, _counter, _settings.SpeedMultiplier);
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


        public bool HeartBeat()
        {
            if (_aggregator.Error != null)
                throw _aggregator.Error;

            _errorHandler.Assert();
            // ReplayScheduler stops threads when IDataReader rans out of replay items.
            return _counter.CreatedThreadCount == 0;
        }

        public void Stop()
        {
            _pool?.StopAndDispose((int)_settings.FinishTimeout.TotalMilliseconds);
            _pool = null;

            _timer.Stop();

            _dataReader.End();
            _aggregator.End();

            _errorHandler.Assert();
        }
    }
}