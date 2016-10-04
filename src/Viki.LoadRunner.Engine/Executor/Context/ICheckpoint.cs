using System;

namespace Viki.LoadRunner.Engine.Executor.Context
{

    /// <summary>
    /// Checkpoint holds information of one time meassurement and or thrown error
    /// </summary>
    public interface ICheckpoint
    {
        /// <summary>
        /// Name of checkpoint
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Timepint of iteration when checkpoint was taken
        /// </summary>
        TimeSpan TimePoint { get; }

        /// <summary>
        /// If executed code below checkpoint creation throws error.
        /// Last previously created checkpoint will have this property set with thrown exception.
        /// </summary>
        Exception Error { get; }
    }
}