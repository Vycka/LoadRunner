using System;
using System.Threading;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Scenario;

//#pragma warning disable 1591

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    /// <summary>
    /// Executes work provided with IWork 
    /// Worker-Thread once StartThread()'ed, will initialize ASAP.
    /// After initialization, it will start execution of iterations ASAP but not until ITimer is started.
    /// </summary>
    public class WorkerThread : IWorkerThread
    {
        #region Fields

        private readonly IWork _work;
        private readonly Thread _handlerThread;
        private bool _stopQueued;

        #endregion

        #region Properties

        public bool Initialized { get; private set; }

        #endregion

        #region Ctor

        public WorkerThread(IWork work)
        {
            _work = work;

            _handlerThread = new Thread(ExecuteScenarioThreadFunction);
        }

        #endregion

        #region Thread Control Functions

        public void StartThread()
        {
            if (_handlerThread.IsAlive)
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

        public event WorkerThreadDelegates.ThreadInitializedEvent ThreadInitialized;

        private void OnThreadInitialized()
        {
            ThreadInitialized?.Invoke(this);
        }

        public event WorkerThreadDelegates.ThreadErrorEvent ThreadError;

        private void OnThreadFailed(Exception ex)
        {
            ThreadError?.Invoke(this, ex);
        }

        public event WorkerThreadDelegates.ThreadStoppedEvent ThreadStopped;

        private void OnThreadFinished()
        {
            ThreadStopped?.Invoke(this);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            StopThread(0);
        }

        #endregion

        #region Thread Function

        private void ExecuteScenarioThreadFunction()
        {
            try
            {
                _work.Init();
                Initialized = true;
                OnThreadInitialized();

                _work.Wait();

                while (!_stopQueued)
                {
                    _work.Execute();
                }

                _work.Cleanup();

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