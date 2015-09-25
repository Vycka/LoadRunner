using System;
using System.Linq;

namespace Viki.LoadRunner.Engine.Executor.Context
{
    public class TestContextResult : IIterationMetadata
    {
        public readonly Checkpoint[] Checkpoints;

        public int GlobalIterationId { get; }
        public int ThreadIterationId { get; }
        public int ThreadId { get; }
        public readonly DateTime IterationStarted;
        public readonly DateTime IterationFinished;

        public int CreatedThreads { get; private set; }
        public int WorkingThreads { get; private set; }

        public TestContextResult(TestContext testContext)
        {
            Checkpoints = testContext.LoggedCheckpoints.ToArray();

            ThreadId = testContext.ThreadId;
            GlobalIterationId = testContext.GlobalIterationId;
            ThreadIterationId = testContext.ThreadIterationId;

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