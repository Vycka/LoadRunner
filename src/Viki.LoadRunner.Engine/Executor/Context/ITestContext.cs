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
        /// Current execution time
        /// </summary>
        TimeSpan ExecutionTime { get; }

        /// <summary>
        /// Unique execution number
        /// </summary>
        int IterartionId { get; }

        int ThreadId { get; }

        ///// <summary>
        ///// Get list of currently logged time checkpoints within this iteration
        ///// </summary>
        //IReadOnlyList<Checkpoint> LoggedCheckpoints { get; }

        ///// <summary>
        ///// Get list of currently logged exceptions caught within this iteration
        ///// </summary>
        //IReadOnlyList<Exception> LoggedExceptions { get; }
    }
}