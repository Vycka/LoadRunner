using System;
using System.Collections.Concurrent;
using Viki.LoadRunner.Engine.Client;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public class ThreadCoordinator : IDisposable
    {
        #region Fields

        private readonly int _maxThreads;
        private readonly Type _testScenarioType;

        private ConcurrentBag<TestExecutorThread> _allThreads = new ConcurrentBag<TestExecutorThread>(); 
        private ConcurrentQueue<TestExecutorThread> _availableThreads = new ConcurrentQueue<TestExecutorThread>();


        private bool _disposing = false;
        private int _nextIterationId = 0;

        #endregion

        #region Properties

        public int AvailableThreadCount => (_maxThreads - _allThreads.Count) + _availableThreads.Count;

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

            InitializeThreads(minThreads);
        }

        #endregion

        #region Methods

        public bool ExecuteTestScenario()
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

        private void InitializeThreads(int threadCount)
        {
            for (int i = 0; i < threadCount; i++)
            {
                int nextThreadId = _allThreads.Count + 1;
                var testScenarioInstance = (ILoadTestScenario)Activator.CreateInstance(_testScenarioType);
                var executorThread = new TestExecutorThread(testScenarioInstance, nextThreadId);

                executorThread.ScenarioExecutionFinished += ExecutorThread_ScenarioExecutionFinished;

                _availableThreads.Enqueue(executorThread);
                _allThreads.Add(executorThread);
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

            _allThreads = null;
            _availableThreads = null;
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
            _availableThreads.Enqueue(sender);

            if (!_disposing)
            {
                OnScenarioExecutionFinished(result);
            }
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