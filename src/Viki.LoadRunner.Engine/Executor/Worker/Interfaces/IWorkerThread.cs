using System;

namespace Viki.LoadRunner.Engine.Executor.Worker.Interfaces
{
    public interface IWorkerThread : IDisposable
    {
        void StartThread();
        void QueueStopThreadAsync();
        void StopThread(int timeoutMilliseconds);

        bool Initialized { get; }

        event WorkerThreadDelegates.ThreadInitializedEvent ThreadInitialized;
        event WorkerThreadDelegates.ThreadErrorEvent ThreadError;
        event WorkerThreadDelegates.ThreadStoppedEvent ThreadStopped;
    }

    public static class WorkerThreadDelegates
    {
        public delegate void ThreadInitializedEvent(WorkerThread sender);

        public delegate void ThreadErrorEvent(WorkerThread sender, Exception ex);

        public delegate void ThreadStoppedEvent(WorkerThread sender);
    }
}