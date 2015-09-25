using System;

namespace Viki.LoadRunner.Engine.Executor.Context
{
    public interface ITestContext : IIterationMetadata
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
    }
}