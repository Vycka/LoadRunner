using System;
using System.Collections.Generic;

namespace Viki.LoadRunner.Engine.Executor.Context
{
    public class TestContext : ITestContext
    {
        #region Fields

        private readonly List<Checkpoint> _checkpoints = new List<Checkpoint>();

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
        }

        internal void Stop()
        {
            IterationFinished = DateTime.UtcNow;
        }

        internal void Reset(int iterationId)
        {
            IterartionId = iterationId;

            _checkpoints.Clear();

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

            Checkpoint newCheckpoint = new Checkpoint(checkpointName, ExecutionTime);
            _checkpoints.Add(newCheckpoint);  
        }

        public TimeSpan ExecutionTime
        {
            get
            {
                if (IterationFinished != DateTime.MinValue)
                    return IterationFinished - IterationStarted;
                if (IterationStarted != DateTime.MaxValue)
                    return DateTime.UtcNow - IterationStarted;

                return TimeSpan.Zero;
            }
        }
        public int IterartionId { get; private set; }
        public int ThreadId { get; }

        #endregion
    }
}