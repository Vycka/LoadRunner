using System;
using System.Linq;

namespace Viki.LoadRunner.Engine.Executor.Context
{
    public class TestContextResult
    {
        public readonly Exception[] Exceptions;
        public readonly Checkpoint[] Checkpoints;

        public readonly int ThreadId;
        public readonly int IterationId;

        public TestContextResult(TestContext testContext)
        {
            Exceptions = testContext.LoggedExceptions.ToArray();
            Checkpoints = testContext.LoggedCheckpoints.ToArray();

            ThreadId = testContext.ThreadId;
            IterationId = testContext.IterartionId;
        }
    }
}