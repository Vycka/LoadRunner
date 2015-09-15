using System;

namespace Viki.LoadRunner.Engine.Executor.Context
{
    public interface ITestContext
    {
        /// <summary>
        /// Marks time checkpoint for current scenario itaration
        /// </summary>
        /// <param name="checkpointName">Checkpoint name</param>
        void Checkpoint(string checkpointName = null);

        /// <summary>
        /// Current timer value of the currently going on iteration.
        /// </summary>
        TimeSpan ExecutionTime { get; }

        /// <summary>
        /// Unique Iteration ID withing all worker-threads (Starts from zero)
        /// [Tip: If scenario fits - this reference ID can be used as index for test-data datasources]
        /// </summary>
        int GlobalIterationId { get; }

        /// <summary>
        /// Unique Iteration ID withing current instance of ILoadTestScenario (Starts from zero)
        /// [Tip: If scenario fits - this reference ID can be used as index for test-data datasources]
        /// </summary>
        int ThreadIterationId { get; }

        /// <summary>
        /// Unique worker-thread ID. It will stay the same throughout all ILoadTestScenario instance lifetime (Starts from zero)
        /// [Tip: If scenario fits - this reference ID can be used as index for test-data datasources]
        /// </summary>
        int ThreadId { get; }


    }
}