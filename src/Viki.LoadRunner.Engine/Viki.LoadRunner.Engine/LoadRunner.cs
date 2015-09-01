using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Viki.LoadRunner.Engine.Aggregator;
using Viki.LoadRunner.Engine.Executor;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine
{
    public class LoadRunner<TTestScenario> where TTestScenario : ITestScenario
    {
        private readonly ExecutionParameters _parameters;
        private readonly ResultsAggregator _resultsAggregator;

        public LoadRunner(ExecutionParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            _parameters = parameters;

            _resultsAggregator = new ResultsAggregator();

        }

        public void Run()
        {
            _resultsAggregator.Reset();

            ThreadCoordinator threadCoordinator = new ThreadCoordinator(_parameters.MinThreads, _parameters.MaxThreads, typeof(TTestScenario));
            threadCoordinator.ScenarioExecutionFinished += _threadCoordinator_ScenarioExecutionFinished;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            TimeSpan lastExecutionQueued = TimeSpan.Zero;
            TimeSpan minimumDelayBetweenTests = TimeSpan.FromTicks(TimeSpan.FromSeconds(1).Ticks / _parameters.MaxRequestsPerSecond);

            while (stopwatch.Elapsed < _parameters.MaxDuration)
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
                } 
            }

            threadCoordinator.Dispose(_parameters.FinishTimeoutMilliseconds);
        }

        public List<ResultItem> QueryResults()
        {
            _resultsAggregator.ProcessResults();
            return _resultsAggregator.BuildResultsObject();
        }

        private void _threadCoordinator_ScenarioExecutionFinished(object sender, TestContextResult result)
        {
            _resultsAggregator.AddResult(result);
        }
    }
}