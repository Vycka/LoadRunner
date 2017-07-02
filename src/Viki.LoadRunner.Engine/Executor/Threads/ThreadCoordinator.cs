using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Viki.LoadRunner.Engine.Executor.Result;

#pragma warning disable 1591

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public class ThreadCoordinator : IDisposable, IThreadPoolStats, IThreadPoolControl
    {
        #region Properties

        public readonly CoordinatorContext Context;

        #endregion

        #region Fields

        private readonly ConcurrentDictionary<int, TestExecutorThread> _allThreads;
        private readonly ConcurrentDictionary<int, TestExecutorThread> _initializedThreads;
        private readonly ConcurrentBag<Exception> _threadErrors;

        private bool _disposing;
        private int _nextIterationId;
        private int _nextThreadId;

        #endregion

        #region Ctor

        public ThreadCoordinator(CoordinatorSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _allThreads = new ConcurrentDictionary<int, TestExecutorThread>();
            _initializedThreads = new ConcurrentDictionary<int, TestExecutorThread>();
            _threadErrors = new ConcurrentBag<Exception>();


            Context = new CoordinatorContext
            {
                Aggregator = settings.Aggregator,
                IdFactory = new IdFactory(),
                Scenario = settings.Scenario,
                Scheduler = settings.Scheduler,
                ThreadPool = this,
                Timer = settings.Timer,
            };
        }

        #endregion

        #region Methods

        public void StopWorkersAsync(int threadCount)
        {
            for (int i = 0; i < threadCount; i++)
            {
                TryRemoveThread(_allThreads.Keys.First());
            }
        }

        public void StartWorkersAsync(int threadCount)
        {
            IEnumerable<TestExecutorThread> newThreads = CreateThreads(threadCount);

            foreach (TestExecutorThread newThread in newThreads)
            {
                newThread.ThreadFailed += ExecutorThread_ThreadFailed;
                newThread.ScenarioSetupSucceeded += NewThread_ScenarioSetupSucceeded;

                newThread.StartThread();

                _allThreads.TryAdd(newThread.ThreadId, newThread);
            }
        }

        public WorkerThreadStats BuildWorkerThreadStats() => new WorkerThreadStats(this);

        private IEnumerable<TestExecutorThread> CreateThreads(int threadCount)
        {
            for (int i = 0; i < threadCount; i++)
            {
                yield return new TestExecutorThread(Context, _nextThreadId++);
            }
        }

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

            foreach (TestExecutorThread testExecutorThread in _allThreads.Values)
            {
                testExecutorThread.Dispose();
            }
        }

        public void StopAndDispose(int timeoutMilliseconds)
        {
            DateTime timeoutThreshold = DateTime.UtcNow.AddMilliseconds(timeoutMilliseconds);

            foreach (TestExecutorThread testExecutorThread in _allThreads.Values)
            {
                testExecutorThread.QueueStopThreadAsync();
            }

            foreach (TestExecutorThread testExecutorThread in _allThreads.Values)
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

        private void NewThread_ScenarioSetupSucceeded(TestExecutorThread sender)
        {
            if (!_disposing && !sender.QueuedToStop)
            {
                _initializedThreads.TryAdd(sender.ThreadId, sender);
            }
        }

        private void ExecutorThread_ThreadFailed(TestExecutorThread sender, IterationResult result, Exception ex)
        {
            if (!_disposing)
            {
                TryRemoveThread(sender.ThreadId);

                _threadErrors.Add(ex);
            }
        }

        private void TryRemoveThread(int threadId)
        {
            TestExecutorThread removedThread;

            _allThreads.TryRemove(threadId, out removedThread);

            removedThread.QueueStopThreadAsync();

            _initializedThreads.TryRemove(threadId, out removedThread);
        }

        #endregion

        #region IThreadPoolStats

        public int CreatedThreadCount => _allThreads.Count;
        public int InitializedThreadCount => _initializedThreads.Count;


        #endregion
    }

    public struct WorkerThreadStats : IThreadPoolStats
    {
        private readonly short _createdThreadCount;
        private readonly short _initializedTheadCount;

        public int CreatedThreadCount => _createdThreadCount;
        public int InitializedThreadCount => _initializedTheadCount;

        public WorkerThreadStats(IThreadPoolStats reference)
        {
            _createdThreadCount = (short)reference.CreatedThreadCount;
            _initializedTheadCount = (short)reference.InitializedThreadCount;
        }
    }
}