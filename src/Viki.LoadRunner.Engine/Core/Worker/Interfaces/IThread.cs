using System;

namespace Viki.LoadRunner.Engine.Core.Worker.Interfaces
{
    public interface IThread : IDisposable
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
        public delegate void ThreadInitializedEvent(IThread sender);

        public delegate void ThreadErrorEvent(IThread sender, Exception ex);

        public delegate void ThreadStoppedEvent(IThread sender);
    }
}