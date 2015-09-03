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

        internal void Start()
        {
            IterationStarted = DateTime.UtcNow;
            _stopwatch.Start();
        }

        internal void Stop()
        {
            _stopwatch.Stop();
            IterationFinished = DateTime.UtcNow;
        }

        internal void Reset(int iterationId)
        {
            IterartionId = iterationId;

            _checkpoints.Clear();
            _stopwatch.Reset();
        }

        internal void SetError(Exception error)
        {
            _checkpoints[_checkpoints.Count - 1].Error = error;
        }

        #endregion

        #region ILoadTestContext

        public void Checkpoint(string checkpointName = null)
        {
            if (checkpointName == null)
                checkpointName = $"Checkpoint #{_checkpoints.Count + 1}";

            Checkpoint newCheckpoint = new Checkpoint(checkpointName, _stopwatch.Elapsed);
            _checkpoints.Add(newCheckpoint);  
        }

        public TimeSpan ExecutionTime => _stopwatch.Elapsed;
        public int IterartionId { get; private set; }
        public int ThreadId { get; }

        #endregion
    }
}