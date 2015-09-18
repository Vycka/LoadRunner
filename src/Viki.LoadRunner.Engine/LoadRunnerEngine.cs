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
                _threadCoordinator.ScenarioIterationFinished += _threadCoordinator_ScenarioIterationFinished;
                _threadCoordinator.InitializeThreads(_parameters.ThreadingStrategy.InitialThreadCount, true);
                
                int testIterationCount = 0;
                TimeSpan executionEnqueueThreshold = TimeSpan.Zero;

                _testElapsedTime = TimeSpan.Zero;
                _testBeginTime = DateTime.UtcNow;
                _resultsAggregator.Begin(_testBeginTime);
                
                while (_testElapsedTime <= _parameters.Limits.MaxDuration && testIterationCount < _parameters.Limits.MaxIterationsCount)
                {
                    WorkerThreadStats threadStats = _threadCoordinator.BuildWorkerThreadStats();
                    int allowedWorkingthreadCount = _parameters.ThreadingStrategy.GetAllowedMaxWorkingThreadCount(_testElapsedTime, threadStats);

                    _threadCoordinator.AssertThreadErrors();
                    TryAdjustCreatedThreadCount(threadStats);

                    
                    if (allowedWorkingthreadCount > threadStats.WorkingThreadCount && _testElapsedTime >= executionEnqueueThreshold)
                    {
                        if (_threadCoordinator.TryEnqueueSingleIteration())
                        {
                            executionEnqueueThreshold = CalculateNextExecutionTime(executionEnqueueThreshold);
                            testIterationCount++;
                        }
                        else
                            Thread.Sleep(TimeSpan.FromTicks(5000));
                    }
                    else
                    {
                        Thread.Sleep(TimeSpan.FromTicks(5000));
                    }

                    _testElapsedTime = DateTime.UtcNow - _testBeginTime;
                }
            }
            finally
            {
                _threadCoordinator?.StopAndDispose((int)_parameters.Limits.FinishTimeout.TotalMilliseconds);
                _resultsAggregator.End();
                _threadCoordinator?.AssertThreadErrors();

                _threadCoordinator = null;
            }
        }

        private void TryAdjustCreatedThreadCount(WorkerThreadStats threadStats)
        {
            int allowedCreatedThreadCount = _parameters.ThreadingStrategy.GetAllowedCreatedThreadCount(_testElapsedTime, threadStats);

            if (allowedCreatedThreadCount > threadStats.CreatedThreadCount)
                _threadCoordinator.InitializeThreads(_parameters.ThreadingStrategy.ThreadCreateBatchSize);
            else if (allowedCreatedThreadCount < threadStats.CreatedThreadCount)
                _threadCoordinator.StopWorkersAsync(threadStats.CreatedThreadCount - allowedCreatedThreadCount);
        }

        private TimeSpan CalculateNextExecutionTime(TimeSpan lastExecutionEnqueueThreshold)
        {
            TimeSpan delayBetweenIterations = _parameters.SpeedStrategy.GetDelayBetweenIterations(_testElapsedTime);

            TimeSpan nextExecutionTime = lastExecutionEnqueueThreshold + delayBetweenIterations;
            if (nextExecutionTime < _testElapsedTime)
                nextExecutionTime = _testElapsedTime;

            return nextExecutionTime;
        }

        #endregion

        #region Events

        private void _threadCoordinator_ScenarioIterationFinished(TestContextResult result)
        {
            _resultsAggregator.TestContextResultReceived(result);
        }

        #endregion
    }
}