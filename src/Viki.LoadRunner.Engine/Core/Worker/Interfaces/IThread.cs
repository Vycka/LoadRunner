using System;

namespace Viki.LoadRunner.Engine.Core.Worker.Interfaces
{
    public interface IThread : IDisposable
    {
        void StartThread();
        void QueueStopThreadAsync();
        void StopThread(int timeoutMilliseconds);

        bool Initialized { get; }

        event ThreadInitializedEventDelegate ThreadInitialized;
        event ThreadErrorEventDelegate ThreadError;
        event ThreadStoppedEventDelegate ThreadStopped;
    }

    public delegate void ThreadInitializedEventDelegate(IThread sender);

    public delegate void ThreadErrorEventDelegate(IThread sender, Exception ex);

    public delegate void ThreadStoppedEventDelegate(IThread sender);
}