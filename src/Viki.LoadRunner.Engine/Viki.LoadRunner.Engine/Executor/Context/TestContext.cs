using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Viki.LoadRunner.Engine.Executor.Context
{
    public class TestContext : ITestContext
    {
        #region Properties

        private readonly List<Exception> _exceptions = new List<Exception>();
        private readonly List<Checkpoint> _checkpoints = new List<Checkpoint>();
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public TestContext(int threadId)
        {
            ThreadId = threadId;
            IterartionId = -1;
        }

        public IReadOnlyList<Checkpoint> LoggedCheckpoints => _checkpoints;

        public IReadOnlyList<Exception> LoggedExceptions => _exceptions;

        #endregion

        #region Internal methods

        public void Start(int iterationId)
        {
            IterartionId = iterationId;

            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public void Reset()
        {
            IterartionId = -1;
            _checkpoints.Clear();
            _exceptions.Clear();
            _stopwatch.Reset();
        }

        public void LogException(Exception ex)
        {
            _exceptions.Add(ex);
        }

        #endregion

        #region ILoadTestContext

        public void Checkpoint(string checkpointName = null)
        {
            if (checkpointName == null)
                checkpointName = $"Checkpoint #{_checkpoints.Count + 1}";

            _checkpoints.Add(new Checkpoint(checkpointName, _stopwatch.Elapsed));
        }

        public TimeSpan ExecutionTime => _stopwatch.Elapsed;
        public int IterartionId { get; private set; }
        public int ThreadId { get; }

        #endregion
    }

    public interface ITestContext
    {
        /// <summary>
        /// Marks time checkpoint for current scenario itaration
        /// </summary>
        /// <param name="checkpointName">Checkpoint name</param>
        void Checkpoint(string checkpointName = null);

        /// <summary>
        /// Current execution time
        /// </summary>
        TimeSpan ExecutionTime { get; }

        /// <summary>
        /// Unique execution number
        /// </summary>
        int IterartionId { get; }

        int ThreadId { get; }

        ///// <summary>
        ///// Get list of currently logged time checkpoints within this iteration
        ///// </summary>
        //IReadOnlyList<Checkpoint> LoggedCheckpoints { get; }

        ///// <summary>
        ///// Get list of currently logged exceptions caught within this iteration
        ///// </summary>
        //IReadOnlyList<Exception> LoggedExceptions { get; }
    }

    public class Checkpoint
    {
        #region Consts

        public const string IterationEndCheckpointName = "SYS_ITERATION_END";
        public const string ErrorsCheckpointName = "SYS_ERRORS";
        public const string TotalsCheckpointName = "SYS_TOTAL";

        #endregion

        public readonly string CheckpointName;
        public readonly TimeSpan TimePoint;

        public Checkpoint(string checkpointName, TimeSpan timePoint)
        {
            CheckpointName = checkpointName;
            TimePoint = timePoint;
        }
    }
}