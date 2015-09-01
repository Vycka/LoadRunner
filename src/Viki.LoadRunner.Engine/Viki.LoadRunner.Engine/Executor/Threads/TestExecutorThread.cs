using System;
using System.Threading;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public class TestExecutorThread : IDisposable
    {
        #region Properties

        private readonly Thread _handlerThread;
        private readonly ITestScenario _testScenario;

        private readonly TestContext _testContext;
        private volatile bool _executeIterationQueued = false;
        private volatile int _queuedIterationId = -1;
        private volatile bool _stopQueued = false;

        public int ThreadId => _testContext.ThreadId;

        #endregion

        #region Ctor

        public TestExecutorThread(ITestScenario testScenario, int threadId)
        {
            _testContext = new TestContext(threadId);
            _handlerThread = new Thread(ExecuteScenarioThreadFunction);

            _testScenario = testScenario;

            _handlerThread = new Thread(ExecuteScenarioThreadFunction);

            _handlerThread.Start();
        }

        #endregion

        #region Thread Control Functions

        public void StartThread()
        {
            if (_handlerThread.IsAlive)
                throw new Exception("TestScenarioThread already started");

            _handlerThread.Start();
        }

        public void QueueStopThreadAsync()
        {
            _stopQueued = true;
        }

        public void StopThread(int timeoutMilliseconds)
        {
            if (_handlerThread.IsAlive)
            {
                _stopQueued = true;

                if (_handlerThread.Join(timeoutMilliseconds))
                {
                    _handlerThread.Abort();
                }

                if (_executeIterationQueued)
                {
                    OnScenarioExecutionFinished();
                }
            }
        }
        
        public void QueueIteration(int iteration)
        {
            if (!_handlerThread.IsAlive)
                throw new Exception("TestScenarioThread is not started");

            if (_executeIterationQueued)
                throw new Exception("Iteration execution already queued");

            _queuedIterationId = iteration;
            _executeIterationQueued = true;
        }

        #endregion

        #region Events


        public delegate void ScenarioExecutionStartedEvent(TestExecutorThread sender);
        public event ScenarioExecutionStartedEvent ScenarioExecutionStarted;

        private void OnScenarioExecutionStarted()
        {
            ScenarioExecutionStarted?.Invoke(this);
        }

        public delegate void ScenarioExecutionFinishedEvent(TestExecutorThread sender, TestContextResult result);
        public event ScenarioExecutionFinishedEvent ScenarioExecutionFinished;

        private void OnScenarioExecutionFinished()
        {
            ScenarioExecutionFinished?.Invoke(this, new TestContextResult(_testContext));
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            StopThread(1000);
        }

        #endregion

        #region Thread Func

        private void ExecuteScenarioThreadFunction()
        {
            _testScenario.ScenarioSetup(_testContext);

            while (_stopQueued != true)
            {
                if (_executeIterationQueued)
                {
                    OnScenarioExecutionStarted();

                    bool setupResult = ExecuteWithExceptionHandling(() => _testScenario.IterationSetup(_testContext), _testContext);

                    if (setupResult)
                    {
                        _testContext.Reset();
                        _testContext.Start(_queuedIterationId);

                        ExecuteWithExceptionHandling(() => _testScenario.ExecuteScenario(_testContext), _testContext);

                        _testContext.Checkpoint(Checkpoint.IterationEndCheckpointName);
                        _testContext.Stop();

                        ExecuteWithExceptionHandling(() => _testScenario.IterationTearDown(_testContext), _testContext);
                    }
                    else
                    {
                        _testContext.Checkpoint(Checkpoint.IterationEndCheckpointName);
                        _testContext.Stop();
                    }

                    OnScenarioExecutionFinished();
                    _executeIterationQueued = false;
                }
                else
                {
                    Thread.Sleep(1);
                }
            }

            _testScenario.ScenarioTearDown(_testContext);
        }

        private static bool ExecuteWithExceptionHandling(Action action, TestContext testContext)
        {
            bool result = false;

            try
            {
                action.Invoke();
                result = true;
            }
            catch (Exception ex)
            {
                testContext.LogException(ex);
            }

            return result;
        }

        #endregion

    }
}