using System;
using System.Diagnostics;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scenario
{
    /// <summary>
    /// Checkpoint holds information of timestamp and error of of current iteration.
    /// </summary>
    [DebuggerDisplay("{Name} {Error == null ? 0 : 1}")]
    public class Checkpoint : ICheckpoint
    {
        #region Consts

        public static class Names
        {
            //public static string Setup = "Setup";
            public static string Skip = "Skip";
            //public static string IterationStart = "ITERATION_START";
            //public static string IterationEnd = "ITERATION_END"; 
            //public static string TearDown = "ITERATION_TEARDOWN";

            public static string Setup = "Setup";
            public static string Iteration = "Iteration";
            public static string TearDown = "TearDown";
        }

        /// <summary>
        /// List of system checkpoints appearing not in ExecuteIteration() section.
        /// </summary>
        public static string[] NotMeassuredCheckpoints = { Names.Setup, Names.Skip, Names.TearDown };

        /// <summary>
        /// List of all system checkpoints which engine can spawn.
        /// </summary>
        public static string[] All = { Names.Setup, Names.Iteration, Names.Skip, Names.TearDown };

        #endregion

        #region Properties

        /// <summary>
        /// Name of the checkpoint
        /// System checkpoints use Checkpoint.Names.* values which intentionally can be changed if one desires.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Timestamp of when this checkpoint was made during iteration
        /// </summary>
        public TimeSpan TimePoint { get; set; }

        /// <summary>
        /// Error logged during the iteration or null.
        /// </summary>
        public object Error { get; set; }

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