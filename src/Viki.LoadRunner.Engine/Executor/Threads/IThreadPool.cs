using System;

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public interface IThreadPool : IThreadPoolStats, IThreadPoolControl, IThreadPoolCounter, IDisposable
    {   
    }

    public interface IThreadPoolCounter
    {
        void AddIdle(int count);
        void AddCreated(int count);
    }
}