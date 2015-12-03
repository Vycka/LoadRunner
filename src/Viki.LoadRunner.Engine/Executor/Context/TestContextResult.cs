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
        public object UserData { get; set; }

        public readonly TimeSpan IterationStarted;
        public readonly TimeSpan IterationFinished;

        public int CreatedThreads { get; private set; }
        public int WorkingThreads { get; private set; }

        public TestContextResult(TestContext testContext)
        {
            ThreadId = testContext.ThreadId;
            GlobalIterationId = testContext.GlobalIterationId;
            ThreadIterationId = testContext.ThreadIterationId;
            UserData = testContext.UserData;


            IterationStarted = testContext.IterationStarted;
            IterationFinished = testContext.IterationFinished;

            Checkpoints = testContext.LoggedCheckpoints.ToArray();
        }

        internal void SetInternalMetadata(int createdThreads, int workingThreads)
        {
            CreatedThreads = createdThreads;
            WorkingThreads = workingThreads;
        }


    }
}