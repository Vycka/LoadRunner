using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Viki.LoadRunner.Engine.Executor.Context
{
    public class TestContext : ITestContext
    {
        #region Fields

        private readonly List<Checkpoint> _checkpoints = new List<Checkpoint>();
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public TestContext(int threadId)
        {
            ThreadId = threadId;
            IterartionId = -1;
        }

        #endregion

        public IReadOnlyList<Checkpoint> LoggedCheckpoints => _checkpoints;
        public DateTime IterationStarted { get; private set; }
        public DateTime IterationFinished { get; private set; }

        #region Internal methods

        public void Start()
        {
            Checkpoint(Context.Checkpoint.IterationStartCheckpointName);
            IterationStarted = DateTime.UtcNow;
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
            IterationFinished = DateTime.UtcNow;
            Checkpoint(Context.Checkpoint.IterationEndCheckpointName);
        }

        public void Reset(int iterationId)
        {
            IterartionId = iterationId;

            IterartionId = -1;
            _checkpoints.Clear();
            _stopwatch.Reset();
        }

        public void LogException(Exception ex)
        {
            _checkpoints[_checkpoints.Count - 1].LogError(ex);
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
}