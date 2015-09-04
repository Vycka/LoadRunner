using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Viki.LoadRunner.Engine.Executor.Context
{
    public class TestContext : ITestContext
    {
        #region Fields

        private readonly List<Checkpoint> _checkpoints = new List<Checkpoint>();
        private readonly Stopwatch iterationTimer = new Stopwatch();

        public TestContext(int threadId)
        {
            ThreadId = threadId;
            
            Reset(-1);
        }

        #endregion

        public IReadOnlyList<Checkpoint> LoggedCheckpoints => _checkpoints;
        public DateTime IterationStarted { get; private set; }
        public DateTime IterationFinished { get; private set; }

        #region Internal methods

        internal void Start()
        {
            IterationStarted = DateTime.UtcNow;
            iterationTimer.Start();
        }

        internal void Stop()
        {
            iterationTimer.Stop();
            IterationFinished = DateTime.UtcNow;
        }

        internal void Reset(int iterationId)
        {
            IterartionId = iterationId;

            _checkpoints.Clear();
            iterationTimer.Reset();

            IterationStarted = DateTime.MaxValue;
            IterationFinished = DateTime.MinValue;
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

            Checkpoint newCheckpoint = new Checkpoint(checkpointName, iterationTimer.Elapsed);
            _checkpoints.Add(newCheckpoint);  
        }

        public TimeSpan ExecutionTime => iterationTimer.Elapsed;
        public int IterartionId { get; private set; }
        public int ThreadId { get; }

        #endregion
    }
}