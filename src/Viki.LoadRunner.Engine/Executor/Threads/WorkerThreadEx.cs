using System;
using System.Threading;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Timer;

#pragma warning disable 1591

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    /// <summary>
    /// Executor defines worker-thread logic
    /// Worker-Thread once created, will initialize ASAP.
    /// After initialization, it will start execution of iterations ASAP but not until ITimer is started.
    /// </summary>
    public class WorkerThreadEx : IDisposable
    {
        //private readonly IThreadPoolCounter _counter;

        private readonly Executor _executor;

        private readonly ITimer _timer;

        #region Properties

        private readonly Thread _handlerThread;
        private bool _stopQueued;

        public bool QueuedToStop => _stopQueued;
        public bool IsAlive => _handlerThread.IsAlive;
        public bool Idle { get; private set; } = false;

        #endregion

        #region Ctor

        public WorkerThreadEx(Executor executor, ITimer timer)
        {
            _executor = executor;
            _timer = timer;

            _handlerThread = new Thread(ExecuteScenarioThreadFunction);
        }

        #endregion

        #region Thread Control Functions

        public void StartThread()
        {
            if (IsAlive)
                throw new Exception("TestScenarioThread already started");

            _handlerThread.Start();
        }

        public void QueueStopThreadAsync()
        {
            _stopQueued = true;
        }

        public void StopThread(int timeoutMilliseconds)
        {
            if (_handlerThread.IsAlive)
            {
                _stopQueued = true;

                if (!_handlerThread.Join(timeoutMilliseconds))
                {
                    _handlerThread.Abort();
                }
            }
        }

        #endregion

        #region Events


        public delegate void ScenarioSetupSucceededEvent(WorkerThreadEx sender);
        public event ScenarioSetupSucceededEvent ScenarioSetupSucceeded;

        private void OnScenarioSetupSucceeded()
        {
            ScenarioSetupSucceeded?.Invoke(this);
        }


        public delegate void ThreadFailedEvent(WorkerThreadEx sender, Exception ex);
        public event ThreadFailedEvent ThreadFailed;

        private void OnThreadFailed(Exception ex)
        {
            ThreadFailed?.Invoke(this, ex);
        }

        public delegate void ThreadFinishedEvent(WorkerThreadEx sender);
        public event ThreadFinishedEvent ThreadFinished;

        private void OnThreadFinished()
        {
            ThreadFinished?.Invoke(this);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            StopThread(0);
        }

        #endregion

        #region Thread Func

        private void ExecuteScenarioThreadFunction()
        {
            try
            { 
                //IThreadContext threadContext = new ThreadContext(_context.ThreadPool, _context.Timer, _testContext);
                //Scheduler scheduler = new Scheduler(_context.Speed, threadContext, _context.ThreadPool);

                _executor.Setup();
                OnScenarioSetupSucceeded();

                // Wait for ITimer to start.
                while (_timer.IsRunning == false && _stopQueued == false)
                    Thread.Sleep(1);

                while (!_stopQueued)
                {
                    _executor.Execute();
                }

                _executor.Teardown();

            }
            catch (Exception ex)
            {
                OnThreadFailed(ex);
            }
            finally
            {
                OnThreadFinished();
            }
        }

        public static void ExecuteIteration(TestContext context, ILoadTestScenario scenario)
        {
            context.Checkpoint(Checkpoint.Names.Setup);
            bool setupSuccess = ExecuteWithExceptionHandling(() => scenario.IterationSetup(context), context);

            if (setupSuccess)
            {
                context.Checkpoint(Checkpoint.Names.IterationStart);

                context.Start();
                bool iterationSuccess = ExecuteWithExceptionHandling(() => scenario.ExecuteScenario(context), context);
                context.Stop();

                if (iterationSuccess)
                {
                    context.Checkpoint(Checkpoint.Names.IterationEnd);
                }
            }
            else
            {
                context.Start();
                context.Stop();
            }

            context.Checkpoint(Checkpoint.Names.TearDown);
            ExecuteWithExceptionHandling(() => scenario.IterationTearDown(context), context);
        }


        private static bool ExecuteWithExceptionHandling(Action action, TestContext testContext)
        {
            bool result = false;

            try
            {
                action.Invoke();
                result = true;
            }
            catch (Exception ex)
            {
                testContext.SetError(ex);
            }

            return result;
        }

        #endregion
    }
}