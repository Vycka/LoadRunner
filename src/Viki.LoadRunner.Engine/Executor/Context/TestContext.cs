using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Viki.LoadRunner.Engine.Executor.Context
{
    public class TestContext : ITestContext
    {
        #region Properties

        private readonly List<Checkpoint> _checkpoints = new List<Checkpoint>();
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public TestContext(int threadId)
        {
            ThreadId = threadId;
            IterartionId = -1;
        }

        public IReadOnlyList<Checkpoint> LoggedCheckpoints => _checkpoints;

        #endregion

        #region Internal methods

        public void Start()
        {
            Checkpoint(Context.Checkpoint.IterationStartCheckpointName);
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
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