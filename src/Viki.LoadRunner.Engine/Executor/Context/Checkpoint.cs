using System;
using System.Collections.Generic;

namespace Viki.LoadRunner.Engine.Executor.Context
{
    public class Checkpoint
    {
        #region Consts

        public const string IterationStartCheckpointName = "SYS_ITERATION_START";
        public const string IterationEndCheckpointName = "SYS_ITERATION_END";
        public const string IterationTearDownEndCheckpointName = "SYS_TEARDOWN_END";

        #endregion

        #region Properties


        public readonly string CheckpointName;
        public readonly TimeSpan TimePoint;
        public Exception Error { get; internal set; }

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