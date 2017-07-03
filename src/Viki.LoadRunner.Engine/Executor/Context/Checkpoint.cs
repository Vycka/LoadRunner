using System;
using System.Diagnostics;

namespace Viki.LoadRunner.Engine.Executor.Context
{
    /// <summary>
    /// Checkpoint holds information of timestamp and error of of current iteration.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class Checkpoint : ICheckpoint
    {
        #region Consts

        public static class Names
        {
            public const string IterationStart = "SYS_ITERATION_START";
            public const string IterationEnd = "SYS_ITERATION_END";
            public const string Setup = "SYS_ITERATION_SETUP";
            public const string TearDown = "SYS_ITERATION_TEARDOWN";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Name of the checkpoint (Checkpoint has const's of system checkpoints)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Timestamp of when this checkpoint was made durring iteration
        /// </summary>
        public TimeSpan TimePoint { get; set; }

        /// <summary>
        /// Error logged during the iteration or null.
        /// </summary>
        public Exception Error { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Create checkpoint
        /// </summary>
        /// <param name="name">name of the checkpoint</param>
        /// <param name="timePoint">Timestamp of the checkpoint</param>
        public Checkpoint(string name, TimeSpan timePoint)
        {
            Name = name;
            TimePoint = timePoint;
        }

        #endregion
    }
}