using System;
using System.Collections.Generic;

namespace Viki.LoadRunner.Engine.Executor.Scenario.Interfaces
{
    public interface IIteration : IIterationMetadata<object>
    {
        /// <summary>
        /// Marks time checkpoint for current scenario itaration
        /// </summary>
        /// <param name="checkpointName">Checkpoint name</param>
        void Checkpoint(string checkpointName = null);

        /// <summary>
        /// Gets list of unhandled errors in current iteration
        /// </summary>
        IEnumerable<KeyValuePair<string, Exception>> GetErrors();

        /// <summary>
        /// Current timer value of the currently going on iteration.
        /// </summary>
        TimeSpan IterationElapsedTime { get; }
    }
}