using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Viki.LoadRunner.Engine.Executor.Result;
using Viki.LoadRunner.Engine.Executor.Threads.Factory;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Scheduler;
using Viki.LoadRunner.Engine.Executor.Threads.Stats;
using Viki.LoadRunner.Engine.Executor.Threads.Strategy;

#pragma warning disable 1591

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public class ThreadPool : IThreadPool
    {
        #region Properties

        public IThreadPoolContext Context => _context;

        #endregion

        #region Fields

        private readonly ConcurrentDictionary<int, WorkerThread> _allThreads;
        private readonly ConcurrentDictionary<int, WorkerThread> _initializedThreads;
        private readonly ConcurrentBag<Exception> _threadErrors;

        private bool _disposing;
        private int _nextThreadId;
        private readonly ThreadPoolContext _context;

        #endregion

        #region Ctor

        public ThreadPool(ThreadPoolSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _allThreads = new ConcurrentDictionary<int, WorkerThread>();
            _initializedThreads = new ConcurrentDictionary<int, WorkerThread>();
            _threadErrors = new ConcurrentBag<Exception>();


            _context = new ThreadPoolContext
            {
                Aggregator = settings.Aggregator,
                IdFactory = new IdFactory(),
                Scenario = settings.Scenario,
                Speed = settings.SpeedStrategy,
                ThreadPool = this,
                Timer = settings.Timer,
            };
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

            foreach (WorkerThread testExecutorThread in _allThreads.Values)
            {
                testExecutorThread.Dispose();
            }
        }

        public void StopAndDispose(int timeoutMilliseconds)
        {
            DateTime timeoutThreshold = DateTime.UtcNow.AddMilliseconds(timeoutMilliseconds);

            foreach (WorkerThread testExecutorThread in _allThreads.Values)
            {
                testExecutorThread.QueueStopThreadAsync();
            }

            foreach (WorkerThread testExecutorThread in _allThreads.Values)
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

        private void OnScenarioSetupSucceeded(WorkerThread sender)
        {
            if (!_disposing && !sender.QueuedToStop)
            {
                _initializedThreads.TryAdd(sender.ThreadId, sender);
            }
        }

        private void OnThreadFinished(WorkerThread sender)
        {
            AddCreated(-1);
        }

        private void OnThreadFailed(WorkerThread sender, IterationResult result, Exception ex)
        {
            if (ex.GetType() != typeof(ThreadAbortException) && !_disposing)
            {
                TryRemoveThread(sender.ThreadId);

                _threadErrors.Add(ex);
            }
        }

        private void TryRemoveThread(int threadId)
        {
            WorkerThread removedThread;

            _allThreads.TryRemove(threadId, out removedThread);

            removedThread.QueueStopThreadAsync();

            _initializedThreads.TryRemove(threadId, out removedThread);
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
            IEnumerable<WorkerThread> newThreads = CreateThreads(threadCount);

            foreach (WorkerThread newThread in newThreads)
            {
                newThread.ThreadFailed += OnThreadFailed;
                newThread.ScenarioSetupSucceeded += OnScenarioSetupSucceeded;
                newThread.ThreadFinished += OnThreadFinished;

                newThread.StartThread();

                _allThreads.TryAdd(newThread.ThreadId, newThread);
            }

            AddCreated(threadCount);
        }

        private IEnumerable<WorkerThread> CreateThreads(int threadCount)
        {
            for (int i = 0; i < threadCount; i++)
            {
                ScenarioInstance instance = new ScenarioInstance(_context.IdFactory, _context.Scenario, _nextThreadId++, _context.UserData, _context.Timer);

                SpeedStrategyHandler strategyHandler = new SpeedStrategyHandler(_context.Speed, instance.Context, this);
                SchedulerEx scheduler = new SchedulerEx(strategyHandler, this, _context.Timer);

                DataCollector collector = new DataCollector(_context.Aggregator, this);

                Executor executor = new Executor(scheduler, instance, collector);
                WorkerThreadEx worker = new WorkerThreadEx(executor, _context.Timer);

                yield return new WorkerThread(_context, _nextThreadId++);
            }
        }

        #endregion

        #region IThreadPoolStats

        public int CreatedThreadCount => _createdCount;
        public int InitializedThreadCount => _initializedThreads.Count;
        public int IdleThreadCount => _idleCount;

        #endregion

        #region IThreadPoolCounter

        private int _idleCount;
        private int _createdCount;

        public void AddIdle(int count)
        {
            Interlocked.Add(ref _idleCount, count);
        }

        public void AddCreated(int count)
        {
            Interlocked.Add(ref _createdCount, count);
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