using System;

namespace Viki.LoadRunner.Engine.Executor.Context
{
    public class Checkpoint : ICheckpoint
    {
        #region Consts

        public const string IterationStartCheckpointName = "SYS_ITERATION_START";
        public const string IterationEndCheckpointName = "SYS_ITERATION_END";
        public const string IterationSetupCheckpointName = "SYS_ITERATION_SETUP";
        public const string IterationTearDownCheckpointName = "SYS_ITERATION_TEARDOWN";

        #endregion

        #region Properties

        public string Name { get; set; }
        public TimeSpan TimePoint { get; set; }
        public Exception Error { get; set; }

        #endregion

        #region Methods

        public Checkpoint(string name, TimeSpan timePoint)
        {
            Name = name;
            TimePoint = timePoint;
        }

        #endregion
    }
}