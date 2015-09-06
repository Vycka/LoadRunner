using System;
using System.Threading;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Threads;
using Viki.LoadRunner.Engine.Parameters;

namespace Viki.LoadRunner.Engine
{
    public class LoadRunnerEngine
    {
        #region Fields

        private readonly LoadRunnerParameters _parameters;
        private readonly IResultsAggregator _resultsAggregator;
        private readonly Type _iTestScenarioObjeType;

        #region Run() globals

        private DateTime _testBeginTime;
        private TimeSpan _testElapsedTime;
        private ThreadCoordinator _threadCoordinator;

        #endregion

        #endregion

        #region Ctor

        public LoadRunnerEngine(LoadRunnerParameters parameters, Type iTestScenarioObjectType, params IResultsAggregator[] resultsAggregators)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            if (iTestScenarioObjectType == null)
                throw new ArgumentNullException(nameof(iTestScenarioObjectType));

            _parameters = parameters;
            _iTestScenarioObjeType = iTestScenarioObjectType;

            _resultsAggregator = new AsyncResultsAggregator(resultsAggregators);

        }

        public static LoadRunnerEngine Create<TTestScenario>(LoadRunnerParameters parameters, params IResultsAggregator[] resultsAggregators) 
            where TTestScenario : ILoadTestScenario
        {
            return new LoadRunnerEngine(parameters, typeof(TTestScenario), resultsAggregators);
        }

        #endregion

        #region Methods

        public void Run()
        {
            try
            {
                _threadCoordinator = new ThreadCoordinator(_iTestScenarioObjeType);
                _threadCoordinator.ScenarioExecutionFinished += _threadCoordinator_ScenarioExecutionFinished;
                _threadCoordinator.InitializeThreads(_parameters.ThreadingStrategy.InitialThreadCount);

                _resultsAggregator.Begin();

                int testIterationCount = 0;
                TimeSpan lastExecutionQueued = TimeSpan.FromSeconds(-10);

                _testElapsedTime = TimeSpan.Zero;
                _testBeginTime = DateTime.UtcNow;

                while (_testElapsedTime <= _parameters.Limits.MaxDuration && testIterationCount < _parameters.Limits.MaxIterationsCount)
                {
                    _threadCoordinator.AssertThreadErrors();

                    if (_threadCoordinator.IdleThreadCount == 0)
                    {
                        int allowedThreadCount = _parameters.ThreadingStrategy.GetAllowedThreadCount(_testElapsedTime);

                        if (allowedThreadCount > _threadCoordinator.CreatedThreadCount)
                            _threadCoordinator.InitializeThreads(_parameters.ThreadingStrategy.ThreadCreateBatchSize);
                        else
                            Thread.Sleep(1);

                        continue;
                    }

                    TimeSpan delayBetweenIterations = _parameters.SpeedStrategy.GetDelayBetweenIterations(_testElapsedTime);

                    if (_testElapsedTime - lastExecutionQueued > delayBetweenIterations && _threadCoordinator.IdleThreadCount > 0)
                    {
                        if (_threadCoordinator.TryRunSingleIteration())
                        {
                            lastExecutionQueued = _testElapsedTime;
                            testIterationCount++;
                        }
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }

                    _testElapsedTime = DateTime.UtcNow - _testBeginTime;
                }
                
            }
            finally
            {
                _threadCoordinator?.StopAndDispose((int)_parameters.Limits.FinishTimeout.TotalMilliseconds);
                _resultsAggregator.End();

                _threadCoordinator?.AssertThreadErrors();
            }
        }

        #endregion

        #region Events

        private void _threadCoordinator_ScenarioExecutionFinished(ThreadCoordinator sender, TestContextResult result, out bool stopThisThread)
        {
            _resultsAggregator.TestContextResultReceived(result);

            int allowedThreadCount = _parameters.ThreadingStrategy.GetAllowedThreadCount(_testElapsedTime);

            stopThisThread = _threadCoordinator.CreatedThreadCount > allowedThreadCount;
        }

        #endregion
    }
}