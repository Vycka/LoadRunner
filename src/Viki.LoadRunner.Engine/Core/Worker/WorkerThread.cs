using System;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Worker.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Worker
{
    /// <summary>
    /// Executes work provided with IWork 
    /// Worker-Thread once StartThread()'ed, will initialize ASAP.
    /// After initialization, it will start execution of iterations ASAP but not until ITimer is started.
    /// </summary>
    public class WorkerThread : IThread
    {
        #region Fields

        private readonly IWork _work;
        private readonly IPrewait _prewait;
        private readonly Thread _handlerThread;
        private bool _stopQueued;

        #endregion

        #region Properties

        public bool Initialized { get; private set; }

        #endregion

        #region Ctor

        public WorkerThread(IWork work, IPrewait prewait)
        {
            if (prewait == null) throw new ArgumentNullException(nameof(prewait));
            _work = work;
            _prewait = prewait;

            _handlerThread = new Thread(ExecuteScenarioThreadFunction);
        }

        #endregion

        #region Thread Control Functions

        public void StartThread()
        {
            if (_handlerThread.IsAlive)
                throw new Exception("TestScenarioThread already started");

            _stopQueued = false;
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
                QueueStopThreadAsync();

                if (!_handlerThread.Join(timeoutMilliseconds))
                {
                    _handlerThread.Abort();
                }
            }
        }

        #endregion

        #region Events

        public event ThreadInitializedEventDelegate ThreadInitialized;

        private void OnThreadInitialized()
        {
            ThreadInitialized?.Invoke(this);
        }

        public event ThreadErrorEventDelegate ThreadError;

        private void OnThreadError(Exception ex)
        {
            ThreadError?.Invoke(this, ex);
        }

        public event ThreadStoppedEventDelegate ThreadStopped;

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

                _prewait.Wait(ref _stopQueued);

                if (!_stopQueued)
                {
                    _work.Execute(ref _stopQueued);
                }
            }
            catch (Exception ex)
            {
                OnThreadError(ex);
            }
            finally
            {
                try
                {
                    _work.Cleanup();
                }
                catch (Exception ex)
                {
                    OnThreadError(ex);
                }

                OnThreadFinished();
            }
        }

        #endregion
    }
}