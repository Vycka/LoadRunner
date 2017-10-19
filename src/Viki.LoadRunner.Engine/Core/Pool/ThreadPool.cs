using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Core.Counter.Interfaces;
using Viki.LoadRunner.Engine.Core.Pool.Interfaces;
using Viki.LoadRunner.Engine.Core.Worker.Interfaces;

#pragma warning disable 1591

namespace Viki.LoadRunner.Engine.Core.Pool
{
    public class ThreadPool : IThreadPool
    {
        #region Fields

        private readonly IThreadFactory _factory;
        private readonly IThreadPoolCounter _counter;

        private readonly ConcurrentDictionary<IThread, IThread> _allThreads;

        #endregion

        #region Ctor

        public ThreadPool(IThreadFactory factory, IThreadPoolCounter counter)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            if (counter == null)
                throw new ArgumentNullException(nameof(counter));

            _factory = factory;
            _counter = counter;


            _allThreads = new ConcurrentDictionary<IThread, IThread>();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            foreach (IThread testExecutorThread in _allThreads.Values)
            {
                testExecutorThread.Dispose();
            }
        }

        public void StopAndDispose(int timeoutMilliseconds)
        {
            DateTime timeoutThreshold = DateTime.UtcNow.AddMilliseconds(timeoutMilliseconds);

            foreach (IThread testExecutorThread in _allThreads.Values)
            {
                testExecutorThread.QueueStopThreadAsync();
            }

            foreach (IThread testExecutorThread in _allThreads.Values)
            {
                int timeleftTillTimeout = (int) (timeoutThreshold - DateTime.UtcNow).TotalMilliseconds;
                if (timeleftTillTimeout < 0)
                    timeleftTillTimeout = 0;

                testExecutorThread.StopThread(timeleftTillTimeout);
            }

            Dispose();
        }

        #endregion

        #region Events

        private void OnThreadInitialized(IThread sender)
        {
            _counter.AddInitialized(1);
        }

        private void OnThreadStopped(IThread sender)
        {
            if (sender.Initialized)
                _counter.AddInitialized(-1);

            _counter.AddCreated(-1);

            TryRemoveThread(sender);
        }

        private void TryRemoveThread(IThread thread)
        {
            IThread removedThread;

            if (_allThreads.TryRemove(thread, out removedThread))
                removedThread.QueueStopThreadAsync();
        }

        #endregion

        #region IThreadPoolControl

        public void StopWorkersAsync(int threadCount)
        {
            for (int i = 0; i < threadCount; i++)
            {
                TryRemoveThread(_allThreads.Keys.First());
            }
        }

        public void StartWorkersAsync(int threadCount)
        {
            IEnumerable<IThread> newThreads = CreateThreads(threadCount);

            foreach (IThread newThread in newThreads)
            {
                newThread.ThreadInitialized += OnThreadInitialized;
                newThread.ThreadStopped += OnThreadStopped;

                newThread.StartThread();

                _allThreads.TryAdd(newThread, newThread);
            }

            _counter.AddCreated(threadCount);
        }

        private IEnumerable<IThread> CreateThreads(int threadCount)
        {
            for (int i = 0; i < threadCount; i++)
            {
                IThread thread = _factory.Create(); 

                yield return thread;
            }
        }

        #endregion
    }
}