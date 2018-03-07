using System;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Counter;
using Viki.LoadRunner.Engine.Core.Counter.Interfaces;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Pool.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;
using Viki.LoadRunner.Engine.Core.Worker;
using Viki.LoadRunner.Engine.Core.Worker.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Adapter.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Interfaces;
using ThreadPool = Viki.LoadRunner.Engine.Core.Pool.ThreadPool;

namespace Viki.LoadRunner.Tools.Strategy
{
    public interface IWorkFactory
    {
        IWork Create();
    }

    public class GenericStrategySettings
    {
        public ITimerControl Timer = new ExecutionTimer();
        // TODO: Split ThreadFactory into generic one.
        public IThreadFactory ThreadFactory;

        // End[ing]Strategy FinishStrategy ?
        public ILimitStrategy[] Limits;

        public TimeSpan FinishTimeout;

        public IThreadingStrategy Threading;

    }

    public abstract class StrategyBase : IStrategy
    {
        private readonly GenericStrategySettings _settings;

        private ThreadPool _pool;
        private IThreadPoolCounter _counter;
        private IErrorHandler _errorHandler;
        private IThreadingStrategy _threading;

        private IUniqueIdFactory<int> _globalIdFactory;

        private ICounter _errorCounter;
        private ICounter _globalIdCounter; //unique id counter .AsCounter
        private ILimitStrategy _limit;
        private ITestState _state;

        public StrategyBase(GenericStrategySettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _settings = settings;
        }

        public void Start()
        {
            _counter = new ThreadPoolCounter();
            _errorHandler = new ErrorHandler();

            _limit = new LimitsHandler(_settings.Limits);
            _threading = _settings.Threading;

            _pool = new ThreadPool(_settings.ThreadFactory, _counter);
        }

        public bool HeartBeat()
        {
            _errorHandler.Assert();

            _threading.HeartBeat(_pool, _state);

            // Maybe inverse?
            return _limit.StopTest(_state);
        }

        public void Stop()
        {
            _pool?.StopAndDispose((int)_settings.FinishTimeout.TotalMilliseconds);
            _pool = null;

            _settings.Timer.Stop();
            
            _errorHandler.Assert();
        }

        private void InitialThreadingSetup()
        {
            while (_counter.CreatedThreadCount != _counter.InitializedThreadCount)
            {
                Thread.Sleep(100);
                _errorHandler.Assert();
            }
        }
    }
}