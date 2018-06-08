using System;
using System.Threading;
using Viki.LoadRunner.Engine.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine
{
    /// <summary>
    /// ILoadTestScenario executor
    /// </summary>
    public class LoadRunnerEngine : IStrategyExecutorAsync, IStrategyExecutor
    {
        #region Fields

        private readonly IStrategy _strategy;
        private Thread _rootThread;

        #endregion

        #region Properties

        /// <summary>
        /// Is test running
        /// </summary>
        public bool Running { get; private set; }

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
        public LoadRunnerEngine(IStrategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            _strategy = strategy;
        }

        #endregion

        #region IStrategyExecutorAsync

        /// <summary>
        /// Start LoadTest execution on main thread. This blocks until test execution is finished by defined rules if any.
        /// </summary>
        public void Run()
        {
            RunInner();
        }

        /// <summary>
        /// Executes test in seperate thread (non-blocking call)
        /// </summary>
        public void RunAsync()
        {
            if (Running)
                throw new InvalidOperationException("Another instance is already running");

            _rootThread = new Thread(RunInner);
            _rootThread.Start();
        }

        /// <summary>
        /// Cancels Async test execution.
        /// Stops exeucion gracefully with time-out handling.
        /// Aggregated data up to this point won't be lost. 
        /// </summary>
        /// <remarks>Use [Running] property to check when it is finished</remarks>
        public void CancelAsync(bool blocking = false)
        {
            Running = false;

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

        #endregion

        #region RunInner() Stuff

        private void RunInner()
        {
            if (Running)
                throw new InvalidOperationException("Engine is already running");

            Exception = null;
            try
            {
                Running = true;
                _strategy.Start();

                while (_strategy.HeartBeat() == false && Running)
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
                Running = false;
            }
        }

        #endregion
    }
}