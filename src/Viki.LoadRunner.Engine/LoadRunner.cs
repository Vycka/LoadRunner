using System;
using System.Threading;
using Viki.LoadRunner.Engine.Presets.Interfaces;

namespace Viki.LoadRunner.Engine
{
    /// <summary>
    /// ILoadTestScenario executor
    /// </summary>
    public class LoadRunner
    {
        #region Fields

        private readonly IStrategy _strategy;

        private Thread _rootThread;
        private bool _running = false;

        #region Run() globals

        //private readonly ExecutionTimer _timer = new ExecutionTimer();
        //private ThreadPool _pool;
        //private IThreadPoolCounter _counter;

        #endregion

        #endregion

        #region Properties

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
        /// <param name="strategy">Test strategy</param>
        public LoadRunner(IStrategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            _strategy = strategy;
        }

        #endregion

        #region Async/Run()

        /// <summary>
        /// Start LoadTest execution on main thread & blocks till test execution & data aggregation is completed.
        /// </summary>
        public void Run()
        {
            RunInner();
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
            _running = false;

            if (blocking)
                Wait();
        }

        /// <summary>
        /// Waits, till execution is finished gracefully with graceful waiting for ExecutionStrategy and FinishTimeout.
        /// </summary>
        /// <param name="timeOut">timeout time period to wait before returning</param>
        /// <param name="abortOnTimeOut">if execution won't finish within desired timeout, should it be terminated prematurely?</param>
        /// <returns>true - if test execution is stopped (either before timeout or aborted due to [abortOnTimeOut])</returns>
        public bool Wait(TimeSpan timeOut, bool abortOnTimeOut = false)
        {
            bool result = _rootThread.Join(timeOut);
            if (abortOnTimeOut && result == false)
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

        private bool IsLoadEngineRunning => _running;

        #endregion

        #region RunInner() Stuff

        private void RunInner()
        {
            if (_running)
                throw new InvalidOperationException("Engine is already running");

            Exception = null;
            try
            {
                _running = true;
                _strategy.Start();

                while (_strategy.HeartBeat() == false && _running)
                {
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
                _strategy.Stop();
                _running = false;
            }
        }

        #endregion
    }
}