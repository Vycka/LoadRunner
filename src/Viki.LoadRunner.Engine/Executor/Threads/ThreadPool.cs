using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Viki.LoadRunner.Engine.Executor.Threads.Factory;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;

#pragma warning disable 1591

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public class ThreadPool : IThreadPool
    {
        #region Properties

        //public IThreadPoolContext Context => _context;

        #endregion

        #region Fields

        private readonly IThreadFactory _factory;
        private readonly IThreadPoolCounter _counter;

        private readonly ConcurrentDictionary<IWorkerThread, IWorkerThread> _allThreads;

        private readonly ConcurrentBag<Exception> _threadErrors;

        private bool _disposing;

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


            _allThreads = new ConcurrentDictionary<IWorkerThread, IWorkerThread>();
            _threadErrors = new ConcurrentBag<Exception>();
        }

        #endregion

        #region Asserts

        public void AssertThreadErrors()
        {
            if (_threadErrors.Count != 0)
            {
                Exception resultError;
                _threadErrors.TryTake(out resultError);

                if (resultError != null)
                    throw resultError;
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _disposing = true;

            foreach (IWorkerThread testExecutorThread in _allThreads.Values)
            {
                testExecutorThread.Dispose();
            }
        }

        public void StopAndDispose(int timeoutMilliseconds)
        {
            DateTime timeoutThreshold = DateTime.UtcNow.AddMilliseconds(timeoutMilliseconds);

            foreach (IWorkerThread testExecutorThread in _allThreads.Values)
            {
                testExecutorThread.QueueStopThreadAsync();
            }

            foreach (IWorkerThread testExecutorThread in _allThreads.Values)
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

        private void OnThreadInitialized(IWorkerThread sender)
        {
            _counter.AddInitialized(1);
        }

        private void OnThreadStopped(IWorkerThread sender)
        {
            if (sender.Initialized)
                _counter.AddInitialized(-1);

            _counter.AddCreated(-1);

            TryRemoveThread(sender);
        }

        private void OnThreadError(IWorkerThread sender, Exception ex)
        {
            if (ex.GetType() != typeof(ThreadAbortException))
            {
                _threadErrors.Add(ex);
            }
        }

        private void TryRemoveThread(IWorkerThread thread)
        {
            IWorkerThread removedThread;

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
            IEnumerable<IWorkerThread> newThreads = CreateThreads(threadCount);

            foreach (IWorkerThread newThread in newThreads)
            {
                newThread.ThreadError +=  OnThreadError;
                newThread.ThreadInitialized += OnThreadInitialized;
                newThread.ThreadStopped += OnThreadStopped;

                newThread.StartThread();

                _allThreads.TryAdd(newThread, newThread);
            }

            _counter.AddCreated(threadCount);
        }

        private IEnumerable<IWorkerThread> CreateThreads(int threadCount)
        {
            for (int i = 0; i < threadCount; i++)
            {
                IWorkerThread thread = _factory.Create(); 

                yield return thread;
            }
        }

        #endregion
    }

    public struct WorkerThreadStats : IThreadPoolStats
    {
        private readonly short _createdThreadCount;
        private readonly short _initializedTheadCount;
        private readonly short _idleThreadCount;

        public int CreatedThreadCount => _createdThreadCount;
        public int InitializedThreadCount => _initializedTheadCount;
        public int IdleThreadCount => _idleThreadCount;

        public WorkerThreadStats(IThreadPoolStats reference)
        {
            _createdThreadCount = (short)reference.CreatedThreadCount;
            _initializedTheadCount = (short)reference.InitializedThreadCount;
            _idleThreadCount = (short) reference.IdleThreadCount;
        }
    }
}