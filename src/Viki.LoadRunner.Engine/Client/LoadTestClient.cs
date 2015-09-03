using System;
using System.Diagnostics;
using System.Threading;
using Viki.LoadRunner.Engine.Aggregates;
using Viki.LoadRunner.Engine.Executor;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Client
{
    public class LoadTestClient
    {
        #region Fields

        private readonly ExecutionParameters _parameters;
        private readonly AsyncResultsAggregator _resultsAggregator;
        private readonly Type _iTestScenarioObjeType;

        #endregion

        #region Ctor

        public LoadTestClient(ExecutionParameters parameters, Type iTestScenarioObjectType, params IResultsAggregator[] resultsAggregators)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            if (iTestScenarioObjectType == null)
                throw new ArgumentNullException(nameof(iTestScenarioObjectType));

            _parameters = parameters;
            _iTestScenarioObjeType = iTestScenarioObjectType;

            _resultsAggregator = new AsyncResultsAggregator(resultsAggregators);

        }

        public static LoadTestClient Create<TTestScenario>(ExecutionParameters parameters, params IResultsAggregator[] resultsAggregators) where TTestScenario : ILoadTestScenario
        {
            return new LoadTestClient(parameters, typeof(TTestScenario), resultsAggregators);
        }

        #endregion

        #region Methods

        public void Run()
        {
            ThreadCoordinator threadCoordinator = null;
            try
            {
                threadCoordinator = new ThreadCoordinator(_parameters.MinThreads, _parameters.MaxThreads, _iTestScenarioObjeType);
                threadCoordinator.ScenarioExecutionFinished += _threadCoordinator_ScenarioExecutionFinished;

                _resultsAggregator.Start();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                TimeSpan minimumDelayBetweenTests = TimeSpan.FromTicks((int)((TimeSpan.FromSeconds(1).Ticks / _parameters.MaxRequestsPerSecond) + 0.5));
                int testIterationCount = 0;
                TimeSpan lastExecutionQueued = TimeSpan.Zero;

                while (stopwatch.Elapsed < _parameters.MaxDuration  && testIterationCount < _parameters.MaxIterationsCount)
                {
                    if (threadCoordinator.AvailableThreadCount == 0)
                    {
                        Thread.Sleep(1);
                        continue;
                    }

                    if (stopwatch.Elapsed - lastExecutionQueued >= minimumDelayBetweenTests)
                    {
                        lastExecutionQueued = stopwatch.Elapsed;

                        threadCoordinator.ExecuteTestScenario();

                        testIterationCount++;
                    }
                }
            }
            finally
            {
                threadCoordinator?.StopAndDispose(_parameters.FinishTimeoutMilliseconds);

                _resultsAggregator.Stop();
            }
        }

        #endregion

        #region Events

        private void _threadCoordinator_ScenarioExecutionFinished(object sender, TestContextResult result)
        {
            _resultsAggregator.TestContextResultReceived(result);
        }

        #endregion
    }
}