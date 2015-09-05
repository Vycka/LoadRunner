using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public class ThreadCoordinator : IDisposable
    {
        #region Fields

        private readonly int _maxThreads;
        private readonly Type _testScenarioType;

        private readonly ConcurrentBag<TestExecutorThread> _allThreads = new ConcurrentBag<TestExecutorThread>(); 
        private readonly ConcurrentQueue<TestExecutorThread> _availableThreads = new ConcurrentQueue<TestExecutorThread>();
        private readonly ConcurrentBag<Exception> _threadErrors = new ConcurrentBag<Exception>();

        private bool _disposing;
        private int _nextIterationId = 1;
        private int _nextThreadId = 1;

        #endregion

        #region Properties

        public int AvailableThreadCount => (_maxThreads - _allThreads.Count) + _availableThreads.Count;
        public int CreatedThreadCount => _allThreads.Count;
        public int IdleThreadCount => _availableThreads.Count;

        #endregion

        #region Ctor

        public ThreadCoordinator(int minThreads, int maxThreads, Type testScenarioType)
        {
            if (testScenarioType == null)
                throw new ArgumentNullException(nameof(testScenarioType));
            if (minThreads > maxThreads)
                throw new Exception("MinThreadCount must be less or equal to MaxThreadCount");

            _maxThreads = maxThreads;
            _testScenarioType = testScenarioType;

            InitializeThreads(minThreads, true);
        }

        #endregion

        #region Methods

        public bool TryRunSingleIteration()
        {
            bool result = false;
            TestExecutorThread thread = GetFreeThread();

            if (thread != null)
            {
                thread.QueueIteration(_nextIterationId++);
                result = true;
            }

            return result;
        }

        private TestExecutorThread GetFreeThread()
        {
            if (_availableThreads.Count == 0 && _allThreads.Count < _maxThreads)
                InitializeThreads(1);

            TestExecutorThread result;
            _availableThreads.TryDequeue(out result);

            return result;
            
        }

        private void InitializeThreads(int threadCount, bool blockingScenarioSetup = false)
        {
            List<TestExecutorThread> newThreads = CreateThreads(threadCount).ToList();

            foreach (TestExecutorThread newThread in newThreads)
            {
                newThread.ScenarioExecutionFinished += ExecutorThread_ScenarioExecutionFinished;
                newThread.ThreadFailed += ExecutorThread_ThreadFailed;
                newThread.ScenarioSetupSucceeded += NewThread_ScenarioSetupSucceeded;

                newThread.StartThread();

                _allThreads.Add(newThread);
            }

            if (blockingScenarioSetup)
            {
                while (newThreads.Any(t => t.IsAlive && t.ScenarioInitialized == false))
                {
                    Thread.Sleep(50);
                }
            }
        }

        private void NewThread_ScenarioSetupSucceeded(TestExecutorThread sender)
        {
            _availableThreads.Enqueue(sender);
        }

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

            foreach (TestExecutorThread testExecutorThread in _allThreads)
            {
                testExecutorThread.Dispose();
            }
        }

        public void StopAndDispose(int timeoutMilliseconds)
        {
            DateTime timeoutThreshold = DateTime.UtcNow.AddMilliseconds(timeoutMilliseconds);

            foreach (TestExecutorThread testExecutorThread in _allThreads)
            {
                testExecutorThread.QueueStopThreadAsync();
            }

            foreach (TestExecutorThread testExecutorThread in _allThreads)
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

        private void ExecutorThread_ScenarioExecutionFinished(TestExecutorThread sender, TestContextResult result)
        {
            Parallel.Invoke((() =>
            {
                _availableThreads.Enqueue(sender);

                if (!_disposing)
                {
                    result.SetInternalMetadata(CreatedThreadCount, CreatedThreadCount - IdleThreadCount + 1);
                    OnScenarioExecutionFinished(result);
                }
            }));
        }

        private void ExecutorThread_ThreadFailed(TestExecutorThread sender, TestContextResult result, Exception ex)
        {
            Parallel.Invoke((() =>
            {
                sender.QueueStopThreadAsync();

                _threadErrors.Add(ex);

                TestExecutorThread failedThread;
                _allThreads.TryTake(out failedThread);
            }));
        }

        public delegate void ScenarioExecutionFinishedEvent(object sender, TestContextResult result);
        public event ScenarioExecutionFinishedEvent ScenarioExecutionFinished;

        private void OnScenarioExecutionFinished(TestContextResult result)
        {
            ScenarioExecutionFinished?.Invoke(this, result);
        }

        #endregion
    }
}