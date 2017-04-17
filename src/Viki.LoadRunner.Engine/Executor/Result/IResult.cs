using System;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Executor.Result
{

    /// <summary>
    /// IResult defines meassurement data structure
    /// this can be seen as fundamental masterdata.
    /// Only the list of these IResult's are needed for meassured data aggregation.
    /// </summary>
    public interface IResult : IIterationMetadata<object>
    {
        /// <summary>
        /// All checkpoints containing meassurements from whole iteration
        /// </summary>
        ICheckpoint[] Checkpoints { get; }

        /// <summary>
        /// It contains value when this iteration  started (relative to LoadTest start)
        /// </summary>
        TimeSpan IterationStarted { get;  }

        /// <summary>
        /// It contains value when this iteration ended (relative to LoadTest start)
        /// </summary>
        TimeSpan IterationFinished { get; }

        /// <summary>
        /// Count of currently created test scenario runner threads at the end of this iteration
        /// </summary>
        int CreatedThreads { get; }

        /// <summary>
        /// Count of currently working test scenario runner threads at the end of this iteration
        /// </summary>
        int WorkingThreads { get; }
    }
}