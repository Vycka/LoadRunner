using System;
using System.Threading;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Result;

#pragma warning disable 1591

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    /// <summary>
    /// Executor defines worker-thread logic
    /// Worker-Thread once created, will initialize ASAP.
    /// After initialization, it will start execution of iterations ASAP but not until ITimer is started.
    /// </summary>
    public class TestExecutorThread : IDisposable
    {
        private readonly CoordinatorContext _context;

        #region Properties

        private readonly Thread _handlerThread;
        private readonly ILoadTestScenario _scenario;

        private readonly TestContext _testContext;
        private volatile bool _stopQueued;

        public bool QueuedToStop => _stopQueued;
        public bool ScenarioInitialized { get; private set; }
        public int ThreadId => _testContext.ThreadId;
        public bool IsAlive => _handlerThread.IsAlive;

        #endregion

        #region Ctor

        public TestExecutorThread(CoordinatorContext context, int threadId)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;


            _testContext = new TestContext(threadId, _context.Timer, _context.UserData);
            _scenario = (ILoadTestScenario)Activator.CreateInstance(_context.Scenario);

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
                    _handlerThread.Abort();
                }
            }
        }

        #endregion

        #region Events


        public delegate void ScenarioSetupSucceededEvent(TestExecutorThread sender);
        public event ScenarioSetupSucceededEvent ScenarioSetupSucceeded;

        private void OnScenarioSetupSucceeded()
        {
            ScenarioSetupSucceeded?.Invoke(this);
        }

        private void OnScenarioIterationFinished()
        {
            _context.Aggregator.TestContextResultReceived(new IterationResult(_testContext));
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
                IThreadContext threadContext = new ThreadContext(_context.ThreadPool, _context.Timer, _testContext);
                IterationControl control = new IterationControl();

                int threadIterationId = 0;

                ExecuteScenarioSetup();

                // Wait for ITimer to start.
                while (_context.Timer.IsRunning == false && _stopQueued == false)
                    Thread.Sleep(1);

                if (!_stopQueued)
                    _testContext.Reset(threadIterationId++, _context.IdFactory.Next());

                while (_stopQueued == false)
                {
                    _context.Scheduler.Next(threadContext, control);

                    if (control.Action == IterationControl.IterationAction.Idle)
                    {
                        Thread.Sleep(control.TimeValue);
                        continue;
                    }

                    TimeSpan timerValue = _context.Timer.Value;
                    if (control.TimeValue > timerValue)
                        Thread.Sleep(control.TimeValue - timerValue);

                    ExecuteIteration(_testContext, _scenario);

                    OnScenarioIterationFinished();

                    _testContext.Reset(threadIterationId++, _context.IdFactory.Next());
                }

                _testContext.Reset(-1,-1);
                _scenario.ScenarioTearDown(_testContext);
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ThreadAbortException))
                {
                    OnThreadFailed(ex);
                }
            }

        }

        public static void ExecuteIteration(TestContext context, ILoadTestScenario scenario)
        {
            context.Checkpoint(Checkpoint.IterationSetupCheckpointName);
            bool setupSuccess = ExecuteWithExceptionHandling(() => scenario.IterationSetup(context), context);

            if (setupSuccess)
            {
                context.Checkpoint(Checkpoint.IterationStartCheckpointName);

                context.Start();
                bool iterationSuccess = ExecuteWithExceptionHandling(() => scenario.ExecuteScenario(context), context);
                context.Stop();

                if (iterationSuccess)
                {
                    context.Checkpoint(Checkpoint.IterationEndCheckpointName);
                }
            }
            else
            {
                context.Start();
                context.Stop();
            }

            context.Checkpoint(Checkpoint.IterationTearDownCheckpointName);
            ExecuteWithExceptionHandling(() => scenario.IterationTearDown(context), context);
        }

        private void ExecuteScenarioSetup()
        {
            if (ScenarioInitialized == false)
            { 
                _testContext.Reset(-1,-1);
                _scenario.ScenarioSetup(_testContext);
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