using System;
using System.Linq;
using System.Threading;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Executor.Threads;
using Viki.LoadRunner.Engine.Executor.Timer;
using Viki.LoadRunner.Engine.Parameters;
using Viki.LoadRunner.Engine.Settings;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Speed.PriorityStrategy;
using ThreadPool = Viki.LoadRunner.Engine.Executor.Threads.ThreadPool;

namespace Viki.LoadRunner.Engine
{
    /// <summary>
    /// ILoadTestScenario executor
    /// </summary>
    public class LoadRunnerEngine
    {
        #region Fields

        private readonly ILoadRunnerSettings _settings;
        private ILimitStrategy[] _limits;
        private readonly IResultsAggregator _aggregator;

        private Thread _rootThread;

        #region Run() globals

        private readonly ExecutionTimer _timer = new ExecutionTimer();
        private ThreadPool _pool;

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Current duration of currently executing load test
        /// </summary>
        public TimeSpan TestDuration => _timer.Value;
        /// <summary>
        /// Start UTC time for currently executing load test
        /// </summary>
        public DateTime TestBeginTimeUtc => _timer.BeginTime;

        /// <summary>
        /// Indicates whether LoadTest is currently running
        /// </summary>
        public bool IsRunning => _timer.IsRunning;

        /// <summary>
        /// Controls, how often root thread will ping strategies with HeartBeat()
        /// Some strategies depend on this to readjust time sensitive limits, and increasing value too much can result some Thread/Speed precision decrease.
        /// E.g. ListOfSpeed strategy will readjust speed only at this heart-beat.
        /// </summary>
        public int HeartBeatMs = 100;

        /// <summary>
        /// If execution failed due to unhandled exception, it will be set here.
        /// </summary>
        public Exception Exception { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes new executor instance
        /// </summary>
        /// <param name="settings">LoadTest settings</param>
        /// <param name="resultsAggregators">Aggregators to use when aggregating results from all iterations</param>
        public LoadRunnerEngine(ILoadRunnerSettings settings, params IResultsAggregator[] resultsAggregators)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _settings = settings;

            _aggregator = new AsyncResultsAggregator(resultsAggregators);
        }

        #endregion

        #region Async/Run()

        /// <summary>
        /// Start LoadTest execution on separate thread & blocks till test execution & data aggregation is completed.
        /// </summary>
        public void Run()
        {
            RunAsync();
            Wait();
        }

        /// <summary>
        /// Executes load test in seperate thread (non-blocking call)
        /// </summary>
        public void RunAsync()
        {
            if (IsLoadEngineRunning)
                throw new InvalidOperationException("Another instance is already running");

            _rootThread = new Thread(RunInner);
            _rootThread.Start();
        }

        /// <summary>
        /// Cancels Async test execution.
        /// Stops exeucion safely with time-out handling.
        /// Aggregated data up to this point won't be lost. 
        /// </summary>
        public void CancelAsync(bool blocking = true)
        {
            _limits = new ILimitStrategy[]{ new IterationLimit(0)  };

            if (blocking)
                Wait();
        }

        /// <summary>
        /// Waits, till execution is finished.
        /// </summary>
        /// <param name="timeOut">timeout time period to wait before returning</param>
        /// <param name="abortOnTimeOut">if execution won't finish within desired timeout, should it be terminated prematurely?</param>
        /// <returns>true - if test execution is stopped (either before timeout or aborted due to [abortOnTimeOut])</returns>
        public bool Wait(TimeSpan timeOut, bool abortOnTimeOut = false)
        {
            bool result = _rootThread.Join(timeOut);
            if (abortOnTimeOut == true && result == false)
            {
                _rootThread.Abort();
                _rootThread?.Join();
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Waits Infinitely until loadtest execution is finished
        /// </summary>
        public void Wait()
        {
            _rootThread?.Join();
        }

        private bool IsLoadEngineRunning => _rootThread?.IsAlive == true;

        #endregion

        #region RunInner() Stuff

        private void RunInner()
        {
            if (_pool != null)
                throw new InvalidOperationException("Engine is already running");

            Exception = null;

            try
            {
                _limits = _settings.Limits;
                ISpeedStrategy speed = StrategyBuilder.Create(_settings.Speed, _timer);

                _pool = new ThreadPool(new ThreadPoolSettings
                {
                    InitialUserData = _settings.InitialUserData,
                    Scenario = _settings.TestScenarioType,

                    SpeedStrategy = speed,
                    Timer = _timer,

                    Aggregator = _aggregator
                });

                IThreadPoolContext context = _pool.Context;
                IThreadingStrategy threading = _settings.Threading;

                InitialThreadingSetup(_pool, threading);

                StartTest(context, _timer);

                while (!_limits.Any(l => l.StopTest(context)))
                {
                    _pool.AssertThreadErrors();

                    threading.HeartBeat(context, _pool);
                    speed.HeartBeat(context);

                    Thread.Sleep(HeartBeatMs);
                }
            }
            catch (Exception ex)
            {
                Exception = ex;
                throw;
            }
            finally
            {
                _pool?.StopAndDispose((int)_settings.FinishTimeout.TotalMilliseconds);

                _aggregator.End();
                _timer.Stop();

                ThreadPool local = _pool;
                _pool = null;
                local?.AssertThreadErrors();
            }
        }

        private static void StartTest(IThreadPoolContext context, ExecutionTimer timer)
        {
            context.Aggregator.Begin();
            timer.Start(); // This line also releases Worker-Threads from wait.
        }

        private static void InitialThreadingSetup(ThreadPool pool, IThreadingStrategy threading)
        {
            threading.Setup(pool.Context, pool);

            while (pool.CreatedThreadCount != pool.InitializedThreadCount)
            {
                Thread.Sleep(100);
                pool.AssertThreadErrors();
            }
        }

        #endregion
    }
}