using System;
using System.Linq;

namespace Viki.LoadRunner.Engine.Executor.Context
{
    public class TestContextResult
    {
        public readonly Checkpoint[] Checkpoints;

        public readonly int ThreadId;
        public readonly int IterationId;
        public readonly DateTime IterationStarted;
        public readonly DateTime IterationFinished;

        public int CreatedThreads { get; private set; }
        public int WorkingThreads { get; private set; }

        public TestContextResult(TestContext testContext)
        {
            Checkpoints = testContext.LoggedCheckpoints.ToArray();

            ThreadId = testContext.ThreadId;
            IterationId = testContext.IterartionId;

            IterationStarted = testContext.IterationStarted;
            IterationFinished = testContext.IterationFinished;
        }

        internal void SetInternalMetadata(int createdThreads, int workingThreads)
        {
            CreatedThreads = createdThreads;
            WorkingThreads = workingThreads;
        }
    }
}