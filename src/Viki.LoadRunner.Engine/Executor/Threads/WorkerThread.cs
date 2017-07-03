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
    public class WorkerThread : IDisposable
    {
        private readonly ThreadPoolContext _context;

        #region Properties

        private readonly Thread _handlerThread;
        private readonly ILoadTestScenario _scenario;

        private readonly TestContext _testContext;
        private volatile bool _stopQueued;

        public bool QueuedToStop => _stopQueued;
        public bool ScenarioInitialized { get; private set; }
        public int ThreadId => _testContext.ThreadId;
        public bool IsAlive => _handlerThread.IsAlive;
        public bool Idle { get; private set; } = false;

        #endregion

        #region Ctor

        public WorkerThread(ThreadPoolContext context, int threadId)
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


        public delegate void ScenarioSetupSucceededEvent(WorkerThread sender);
        public event ScenarioSetupSucceededEvent ScenarioSetupSucceeded;

        private void OnScenarioSetupSucceeded()
        {
            ScenarioSetupSucceeded?.Invoke(this);
        }

        private void OnScenarioIterationFinished()
        {
            _context.Aggregator.TestContextResultReceived(new IterationResult(_testContext, _context.ThreadPool));
        }

        public delegate void ThreadFailedEvent(WorkerThread sender, IterationResult result, Exception ex);
        public event ThreadFailedEvent ThreadFailed;

        private void OnThreadFailed(Exception ex)
        {
            ThreadFailed?.Invoke(this, new IterationResult(_testContext, _context.ThreadPool), ex);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            StopThread(0);
        }

        #endregion

        #region Thread Func

        private static readonly TimeSpan MilliSecond = TimeSpan.FromMilliseconds(1);

        private void ExecuteScenarioThreadFunction()
        {
            _context.ThreadPool.AddCreated(1);

            try
            { 
                IThreadContext threadContext = new ThreadContext(_context.ThreadPool, _context.Timer, _testContext);
                Scheduler scheduler = new Scheduler(_context.Timer);

                int threadIterationId = 0;

                ExecuteScenarioSetup();

                // Wait for ITimer to start.
                while (_context.Timer.IsRunning == false && _stopQueued == false)
                    Thread.Sleep(1);

                if (!_stopQueued)
                    _testContext.Reset(threadIterationId++, _context.IdFactory.Next());

                while (_stopQueued == false)
                {
                    if (WaitForExecuteCommand(threadContext, scheduler))
                        continue;

                    ExecuteIteration(_testContext, _scenario);

                    OnScenarioIterationFinished();

                    _testContext.Reset(threadIterationId++, _context.IdFactory.Next());
                }

                _testContext.Reset(-1, -1);
                _scenario.ScenarioTearDown(_testContext);
            }
            catch (Exception ex)
            {
                _context.ThreadPool.AddCreated(-1);

                if (ex.GetType() != typeof(ThreadAbortException))
                {
                    OnThreadFailed(ex);
                }
            }
            finally
            {
                _context.ThreadPool.AddCreated(-1);
            }

        }

        private bool WaitForExecuteCommand(IThreadContext context, Scheduler scheduler)
        {
            _context.Scheduler.Next(context, scheduler);

            if (scheduler.At - _context.Timer.Value > MilliSecond)
            {
                _context.ThreadPool.AddIdle(1);

                if (scheduler.Action == Scheduler.ScheduleAction.Idle)
                {
                    while (_stopQueued == false && scheduler.Action == Scheduler.ScheduleAction.Idle)
                    {
                        TimeSpan deltaIdle = scheduler.At - _context.Timer.Value;
                        if (deltaIdle < MilliSecond)
                            Thread.Sleep(deltaIdle);

                        _context.Scheduler.Next(context, scheduler);
                    }

                    if (_stopQueued)
                        return false;
                }

                TimeSpan deltaWait = scheduler.At - _context.Timer.Value;
                Thread.Sleep(deltaWait > MilliSecond ? deltaWait : MilliSecond);

                _context.ThreadPool.AddIdle(-1);
            }

            return true;
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