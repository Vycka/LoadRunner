using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public class ThreadCoordinator : IDisposable
    {
        #region Fields

        private readonly Type _testScenarioType;

        private readonly ConcurrentDictionary<int, TestExecutorThread> _allThreads = new ConcurrentDictionary<int, TestExecutorThread>();
        private readonly ConcurrentDictionary<int, TestExecutorThread> _initializedThreads = new ConcurrentDictionary<int, TestExecutorThread>();
        private readonly ConcurrentQueue<TestExecutorThread> _idleThreads = new ConcurrentQueue<TestExecutorThread>();
        private readonly ConcurrentBag<Exception> _threadErrors = new ConcurrentBag<Exception>();

        private bool _disposing;
        private int _nextIterationId;
        private int _nextThreadId;

        #endregion

        #region Ctor

        public ThreadCoordinator(Type testScenarioType)
        {
            if (testScenarioType == null)
                throw new ArgumentNullException(nameof(testScenarioType));

            _testScenarioType = testScenarioType;
        }

        #endregion

        #region Methods

        public void EnqueueSingleIteration()
        {
            TestExecutorThread thread = DequeueFreeThread();
            if (thread == null)
                throw new InvalidOperationException("Tried to Enqueue iteration without free threads");

            thread.QueueIteration(_nextIterationId++);
        }

        public bool TryEnqueueSingleIteration()
        {
            bool result = false;
            TestExecutorThread thread = DequeueFreeThread();

            if (thread != null)
            {
                thread.QueueIteration(_nextIterationId++);
                result = true;
            }

            return result;
        }

        public void StopWorkersAsync(int threadCount)
        {
            for (int i = 0; i < threadCount; i++)
            {
                TryRemoveThread(_allThreads.Keys.First());
            }
        }

        private TestExecutorThread DequeueFreeThread()
        {
            TestExecutorThread result;
            do
            {
                _idleThreads.TryDequeue(out result);
            } while (result != null && result.QueuedToStop == true);
            

            return result;
        }

        public void InitializeThreads(int threadCount, bool blockingScenarioSetup = false)
        {
            List<TestExecutorThread> newThreads = CreateThreads(threadCount).ToList();

            foreach (TestExecutorThread newThread in newThreads)
            {
                newThread.ScenarioExecutionFinished += ExecutorThread_ScenarioExecutionFinished;
                newThread.ThreadFailed += ExecutorThread_ThreadFailed;
                newThread.ScenarioSetupSucceeded += NewThread_ScenarioSetupSucceeded;

                newThread.StartThread();

                _allThreads.TryAdd(newThread.ThreadId, newThread);
            }

            if (blockingScenarioSetup)
            {
                while (newThreads.Any(t => t.IsAlive && t.ScenarioInitialized == false))
                {
                    Thread.Sleep(50);
                }
            }
        }

        public WorkerThreadStats BuildWorkerThreadStats() => new WorkerThreadStats(_allThreads.Count, _initializedThreads.Count, _idleThreads.Count);

        private IEnumerable<TestExecutorThread> CreateThreads(int threadCount)
        {
            for (int i = 0; i < threadCount; i++)
            {
                var testScenarioInstance = (ILoadTestScenario)Activator.CreateInstance(_testScenarioType);
                yield return new TestExecutorThread(testScenarioInstance, _nextThreadId++);
            }
        }

        public void AssertThreadErrors()
        {
            if (_threadErrors.Count != 0)
            {
                Exception resultError;
                _threadErrors.TryTake(out resultError);
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
                _idleThreads.Enqueue(sender);
                _initializedThreads.TryAdd(sender.ThreadId, sender);
            }
        }

        private void ExecutorThread_ScenarioExecutionFinished(TestExecutorThread sender, TestContextResult result)
        {
            if (!_disposing)
            {
                result.SetInternalMetadata(_allThreads.Count, _initializedThreads.Count - _idleThreads.Count);

                OnScenarioExecutionFinished(result);

                if (!sender.QueuedToStop)
                    _idleThreads.Enqueue(sender);
            }
        }

        private void ExecutorThread_ThreadFailed(TestExecutorThread sender, TestContextResult result, Exception ex)
        {
            if (!_disposing)
            {
                TryRemoveThread(sender.ThreadId);

                _threadErrors.Add(ex);
            }
        }

        public delegate void ScenarioExecutionFinishedEventHandler(TestContextResult result);
        public event ScenarioExecutionFinishedEventHandler ScenarioIterationFinished;

        private void OnScenarioExecutionFinished(TestContextResult result)
        {
            ScenarioIterationFinished?.Invoke(result);
        }

        private void TryRemoveThread(int threadId)
        {
            TestExecutorThread removedThread;

            _allThreads.TryRemove(threadId, out removedThread);

            removedThread.QueueStopThreadAsync();

            _initializedThreads.TryRemove(threadId, out removedThread);
        }

        #endregion
    }

    public struct WorkerThreadStats
    {
        private readonly int _createdThreadCount;
        private readonly int _initializedTheadCount;
        private readonly int _readyThreadCount;

        public int CreatedThreadCount => _createdThreadCount;
        public int WorkingThreadCount => _initializedTheadCount - _readyThreadCount;
        public int InitializedThreadCount => _initializedTheadCount;
        public int ReadyThreadCount => _readyThreadCount;

        public WorkerThreadStats(int createdThreadCount, int initializedTheadCount, int readyThreadCount)
        {
            _createdThreadCount = createdThreadCount;
            _initializedTheadCount = initializedTheadCount;
            _readyThreadCount = readyThreadCount;
        }
    }
}