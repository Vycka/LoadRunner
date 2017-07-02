using System;
using System.Linq;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Executor.Result
{
    public class IterationResult : IResult
    {
        public IterationResult()
        {
        }

        /// <summary>
        /// Checkpoints contain meassured durations
        /// </summary>
        public ICheckpoint[] Checkpoints { get; set; }

        public int GlobalIterationId { get; set; }
        public int ThreadIterationId { get; set; }
        public int ThreadId { get; set; }
        public object UserData { get; set; }

        public TimeSpan IterationStarted { get; set; }
        public TimeSpan IterationFinished { get; set; }


        public IterationResult(TestContext testContext)
        {
            ThreadId = testContext.ThreadId;
            GlobalIterationId = testContext.GlobalIterationId;
            ThreadIterationId = testContext.ThreadIterationId;
            UserData = testContext.UserData;


            IterationStarted = testContext.IterationStarted;
            IterationFinished = testContext.IterationFinished;

            Checkpoints = testContext.LoggedCheckpoints.Select(c => c).Cast<ICheckpoint>().ToArray();
        }
    }
}