using System;
using System.Collections.Generic;

namespace Viki.LoadRunner.Engine.Executor.Context
{
    public class Checkpoint
    {
        #region Consts

        public const string IterationStartCheckpointName = "SYS_ITERATION_START";
        public const string IterationEndCheckpointName = "SYS_ITERATION_END";

        #endregion

        #region Fields

        private List<Exception> _errors = null;

        #endregion

        #region Properties


        public readonly string CheckpointName;
        public readonly TimeSpan TimePoint;
        public IReadOnlyList<Exception> Errors => _errors;

        #endregion

        #region Methods

        public Checkpoint(string checkpointName, TimeSpan timePoint)
        {
            CheckpointName = checkpointName;
            TimePoint = timePoint;
        }

        public void LogError(Exception ex)
        {
            if (_errors == null)
                _errors = new List<Exception>();

            _errors.Add(ex);
        }

        #endregion
    }
}