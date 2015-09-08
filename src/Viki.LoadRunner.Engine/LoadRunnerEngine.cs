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

        #region Run() Stuff

        public void Run()
        {
            try
            {
                _threadCoordinator = new ThreadCoordinator(_iTestScenarioObjeType);
                _threadCoordinator.ScenarioExecutionFinished += _threadCoordinator_ScenarioExecutionFinished;
                _threadCoordinator.InitializeThreads(_parameters.ThreadingStrategy.InitialThreadCount, true);
                
                int testIterationCount = 0;
                TimeSpan executionEnqueueThreshold = TimeSpan.Zero;

                _testElapsedTime = TimeSpan.Zero;
                _testBeginTime = DateTime.UtcNow;
                _resultsAggregator.Begin(_testBeginTime);
                
                while (_testElapsedTime <= _parameters.Limits.MaxDuration && testIterationCount < _parameters.Limits.MaxIterationsCount)
                {
                    _threadCoordinator.AssertThreadErrors();

                    if (_threadCoordinator.IdleThreadCount == 0)
                    {
                        TryIncreaseWorkerThreadCount();
                    }

                    if (_testElapsedTime >= executionEnqueueThreshold && _threadCoordinator.IdleThreadCount > 0)
                    {
                        if (_threadCoordinator.TryRunSingleIteration())
                        {
                            executionEnqueueThreshold = CalculateNextExecutionTime(executionEnqueueThreshold);
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

        private void TryIncreaseWorkerThreadCount()
        {
            int allowedThreadCount = _parameters.ThreadingStrategy.GetAllowedThreadCount(_testElapsedTime);

            if (allowedThreadCount > _threadCoordinator.CreatedThreadCount)
                _threadCoordinator.InitializeThreads(_parameters.ThreadingStrategy.ThreadCreateBatchSize);
            else
                Thread.Sleep(1);
        }

        private TimeSpan CalculateNextExecutionTime(TimeSpan lastExecutionEnqueueThreshold)
        {
            TimeSpan result;

            TimeSpan delayBetweenIterations = _parameters.SpeedStrategy.GetDelayBetweenIterations(_testElapsedTime);

            if (lastExecutionEnqueueThreshold.Ticks + (delayBetweenIterations.Ticks * 3) > _testElapsedTime.Ticks)
                result = lastExecutionEnqueueThreshold + delayBetweenIterations;
            else
                result = _testElapsedTime;

            return result;
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