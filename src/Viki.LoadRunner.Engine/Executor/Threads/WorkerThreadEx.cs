using System;
using System.Threading;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Threads.Scenario;
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

        private readonly ScenarioWorkerTask _task;

        #region Properties

        private readonly Thread _handlerThread;
        private bool _stopQueued;

        public bool QueuedToStop => _stopQueued;
        public bool IsAlive => _handlerThread.IsAlive;
        public bool Idle { get; private set; } = false;

        #endregion

        #region Ctor

        public WorkerThreadEx(ScenarioWorkerTask task)
        {
            _task = task;

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

                _task.Init();
                OnScenarioSetupSucceeded();

                // Wait for ITimer to start.
                _task.Wait();

                while (!_stopQueued)
                {
                    _task.Execute();
                }

                _task.Cleanup();

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

        #endregion
    }
}