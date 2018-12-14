using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scenario.Interfaces
{
    public interface IIteration : IIterationMetadata<object>
    {
        /// <summary>
        /// Marks time checkpoint for current scenario itaration
        /// </summary>
        /// <param name="checkpointName">Checkpoint name</param>
        void Checkpoint(string checkpointName = null);

        /// <summary>
        /// Set error onto current checkpoint.
        /// Engine will also uses this to set exception if one is thrown and will overwrite existing data.
        /// </summary>
        /// <param name="error">error object to set (setting to null clears it)</param>
        void SetError(object error);

        /// <summary>
        /// Current timer value of the currently going on iteration.
        /// </summary>
        TimeSpan IterationElapsedTime { get; }

        /// <summary>
        /// Root test timer.
        /// </summary>
        ITimer Timer { get; }
    }
}