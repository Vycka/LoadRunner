using System;
using System.Linq;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Pool.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Collector
{
    public class IterationResult : IResult
    {
        public IterationResult()
        {
        }

        public ICheckpoint[] Checkpoints { get; set; }

        public int GlobalIterationId { get; set; }
        public int ThreadIterationId { get; set; }
        public int ThreadId { get; set; }
        public object UserData { get; set; }

        public TimeSpan IterationStarted { get; set; }
        public TimeSpan IterationFinished { get; set; }

        public int CreatedThreads { get; set; }
        public int IdleThreads { get; set; }

        public IterationResult(IIterationResult iteration, IThreadPoolStats threadPoolContext)
        {
            ThreadId = iteration.ThreadId;
            GlobalIterationId = iteration.GlobalIterationId;
            ThreadIterationId = iteration.ThreadIterationId;
            UserData = iteration.UserData;


            IterationStarted = iteration.IterationStarted;
            IterationFinished = iteration.IterationFinished;

            CreatedThreads = threadPoolContext.CreatedThreadCount;
            IdleThreads = threadPoolContext.IdleThreadCount;

            Checkpoints = iteration.Checkpoints.ToArray();
        }
    }
}