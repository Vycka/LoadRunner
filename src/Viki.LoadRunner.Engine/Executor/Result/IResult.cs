using System;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Executor.Result
{
    public interface IResult : IIterationMetadata
    {
        ICheckpoint[] Checkpoints { get; }

        TimeSpan IterationStarted { get;  }
        TimeSpan IterationFinished { get; }

        int CreatedThreads { get; }
        int WorkingThreads { get; }
    }
}