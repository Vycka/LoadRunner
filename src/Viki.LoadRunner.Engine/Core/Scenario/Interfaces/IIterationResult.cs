using System;
using System.Collections.Generic;

namespace Viki.LoadRunner.Engine.Core.Scenario.Interfaces
{
    public interface IIterationResult : IIterationMetadata<object>
    {
        /// <summary>
        /// All checkpoints containing meassurements from whole iteration
        /// </summary>
        ICheckpoint[] CopyCheckpoints();

        /// <summary>
        /// It contains value when this iteration  started (relative to LoadTest start)
        /// </summary>
        TimeSpan IterationStarted { get; }

        /// <summary>
        /// It contains value when this iteration ended (relative to LoadTest start)
        /// </summary>
        TimeSpan IterationFinished { get; }
    }
}