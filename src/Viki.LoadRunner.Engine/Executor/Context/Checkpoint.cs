using System;

namespace Viki.LoadRunner.Engine.Executor.Context
{
    public class Checkpoint
    {
        #region Consts

        public const string IterationStartCheckpointName = "SYS_ITERATION_START";
        public const string IterationEndCheckpointName = "SYS_ITERATION_END";
        public const string IterationSetupCheckpointName = "SYS_ITERATION_SETUP";
        public const string IterationTearDownCheckpointName = "SYS_ITERATION_TEARDOWN";

        #endregion

        #region Properties


        public readonly string CheckpointName;
        public readonly TimeSpan TimePoint;
        internal Exception Error { get; set; }

        #endregion

        #region Methods

        public Checkpoint(string checkpointName, TimeSpan timePoint)
        {
            CheckpointName = checkpointName;
            TimePoint = timePoint;
        }

        #endregion
    }
}