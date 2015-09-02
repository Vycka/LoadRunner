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

        public TestContextResult(TestContext testContext)
        {
            Checkpoints = testContext.LoggedCheckpoints.ToArray();

            ThreadId = testContext.ThreadId;
            IterationId = testContext.IterartionId;

            IterationStarted = testContext.IterationStarted;
            IterationFinished = testContext.IterationFinished;
        }
    }
}