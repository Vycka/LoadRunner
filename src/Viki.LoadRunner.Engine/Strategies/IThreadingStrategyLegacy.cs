using System;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies
{
    public interface IThreadingStrategyLegacy
    {
        /// <summary>
        /// Initial count of threads to be initialized (And ScenarioSetup() executed) before LoadTest is started
        /// </summary>
        int InitialThreadCount { get; }

        /// <summary>
        /// Amount of threads to create each time when created thread count is less than GetAllowedCreatedThreadCount()
        /// </summary>
        /// 
        int ThreadCreateBatchSize { get; }
        //int GetThreadCreateBatchSize(TimeSpan testExecutionTime);

        /// <summary>
        /// Max allowed working thread count. (It can be smaller than GetAllowedCreatedThreadCount)
        /// This allows scenarios, where each ILoadTestScenario instance would represent different user,
        /// and then it's required that only 5 users at the time would do requests, but have these calls spread through 10 users (10 created threads)
        /// </summary>
        int GetAllowedMaxWorkingThreadCount(TimeSpan testExecutionTime, WorkerThreadStats workerThreadStats);

        // TODO: Bug: infinite timeout on threads, which were stopped in the middle of test, won't be aborted, since they wont exist in _allThreads (if they fail to finish them selves)
        /// <summary>
        /// Get Allowed count of threads to be created
        /// * Increasing its value, new threads will be created.
        /// * Decreasing its value, existing threads will be stopped and disposed
        /// </summary>
        int GetAllowedCreatedThreadCount(TimeSpan testExecutionTime, WorkerThreadStats workerThreadStats);
    }
}