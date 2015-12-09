using System;
using System.Threading;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Result;
using Viki.LoadRunner.Engine.Executor.Timer;
#pragma warning disable 1591

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public class TestExecutorThread : IDisposable
    {
        #region Properties

        private readonly Thread _handlerThread;
        private readonly ILoadTestScenario _loadTestScenario;

        private readonly TestContext _testContext;
        private volatile bool _executeIterationQueued;
        private volatile int _queuedIterationId = -1;
        private volatile bool _stopQueued;

        public bool QueuedToStop => _stopQueued;
        public bool ScenarioInitialized { get; private set; }
        public int ThreadId => _testContext.ThreadId;
        public bool IsAlive => _handlerThread.IsAlive;

        #endregion

        #region Ctor

        public TestExecutorThread(ILoadTestScenario loadTestScenario, ITimer timer, int threadId, object initialUserData)
        {
            if (loadTestScenario == null)
                throw new ArgumentNullException(nameof(loadTestScenario));
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));

            _testContext = new TestContext(threadId, timer, initialUserData);
            _loadTestScenario = loadTestScenario;

            _handlerThread = new Thread(ExecuteScenarioThreadFunction);
        }

        #endregion

        #region Thread Control Functions

        public void StartThread()
        {
            if (IsAlive)
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

                if (!_handlerThread.Join(timeoutMilliseconds))
                {
                    //Console.WriteLine($"Aborting {_testContext.ThreadId}");
                    _handlerThread.Abort();

                    ////Broadcast threads that managed to start TeardownProcess
                    //if (_executeIterationQueued && _testContext.ExecutionTime.Ticks != 0)
                    //{
                    //    //Console.WriteLine($"Broadcasting {_testContext.ThreadId}");
                    //    OnScenarioExecutionFinished();
                    //}
                }
            }
        }
        
        public void QueueIteration(int iteration)
        {
            if (!IsAlive)
                throw new Exception("TestScenarioThread is not started");

            if (_executeIterationQueued)
                throw new Exception("Iteration execution already queued");

            _queuedIterationId = iteration;
            _executeIterationQueued = true;
        }

        #endregion

        #region Events


        public delegate void ScenarioSetupSucceededEvent(TestExecutorThread sender);
        public event ScenarioSetupSucceededEvent ScenarioSetupSucceeded;

        private void OnScenarioSetupSucceeded()
        {
            ScenarioSetupSucceeded?.Invoke(this);
        }

        public delegate void ScenarioIterationFinishedEvent(TestExecutorThread sender, IterationResult result);
        public event ScenarioIterationFinishedEvent ScenarioIterationFinished;

        private void OnScenarioIterationFinished()
        {
            ScenarioIterationFinished?.Invoke(this, new IterationResult(_testContext));
        }

        public delegate void ThreadFailedEvent(TestExecutorThread sender, IterationResult result, Exception ex);
        public event ThreadFailedEvent ThreadFailed;

        private void OnThreadFailed(Exception ex)
        {
            ThreadFailed?.Invoke(this, new IterationResult(_testContext), ex);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            StopThread(0);
        }

        #endregion

        #region Thread Func

        private void ExecuteScenarioThreadFunction()
        {
            try
            {
                int threadIterationId = 0;

                ExecuteScenarioSetup();

                while (_stopQueued == false || _executeIterationQueued)
                {
                    if (_executeIterationQueued)
                    {
                        _testContext.Reset(threadIterationId++, _queuedIterationId);

                        _testContext.Checkpoint(Checkpoint.IterationSetupCheckpointName);
                        bool setupSuccess = ExecuteWithExceptionHandling(() => _loadTestScenario.IterationSetup(_testContext), _testContext);

                        if (setupSuccess)
                        {
                            _testContext.Checkpoint(Checkpoint.IterationStartCheckpointName);

                            _testContext.Start();
                            bool iterationSuccess = ExecuteWithExceptionHandling(() => _loadTestScenario.ExecuteScenario(_testContext), _testContext);
                            _testContext.Stop();

                            if (iterationSuccess)
                            {
                                _testContext.Checkpoint(Checkpoint.IterationEndCheckpointName);
                            }
                        }
                        else
                        {
                            _testContext.Start();
                            _testContext.Stop();
                        }

                        _testContext.Checkpoint(Checkpoint.IterationTearDownCheckpointName);
                        ExecuteWithExceptionHandling(() => _loadTestScenario.IterationTearDown(_testContext), _testContext);

                        _executeIterationQueued = false;
                        OnScenarioIterationFinished();
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }

                _testContext.Reset(-1,-1);
                _loadTestScenario.ScenarioTearDown(_testContext);
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ThreadAbortException))
                {
                    OnThreadFailed(ex);
                }
            }

        }

        private void ExecuteScenarioSetup()
        {
            if (ScenarioInitialized == false)
            { 
                _testContext.Reset(-1,-1);
                _loadTestScenario.ScenarioSetup(_testContext);
                ScenarioInitialized = true;
                OnScenarioSetupSucceeded();
            }
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
                testContext.SetError(ex);
            }

            return result;
        }

        #endregion

    }
}