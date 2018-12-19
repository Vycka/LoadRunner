using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Collector.Interfaces
{

    /// <summary>
    /// IResult defines meassurement data structure
    /// this can be seen as fundamental raw data.
    /// Only the list of these IResult's are needed for meassured data aggregation.
    /// </summary>
    public interface IResult : IIterationMetadata<object>
    {
        /// <summary>
        /// Checkpoints with measurements in this iteration
        /// </summary>
        ICheckpoint[] Checkpoints { get; }

        /// <summary>
        /// Count of currently created worker threads at the end of this iteration
        /// This value will be set at the [IterationFinished] moment.
        /// </summary>
        int CreatedThreads { get; }

        /// <summary>
        /// Count of currently idle threads at the end of this iteration
        /// This value will be set at the [IterationFinished] moment.
        /// </summary>
        int IdleThreads { get; }

        /// <summary>
        /// It contains value when this iteration  started (relative to LoadTest start)
        /// </summary>
        TimeSpan IterationStarted { get; }

        /// <summary>
        /// It contains value when this iteration ended (relative to LoadTest start)
        /// </summary>
        TimeSpan IterationFinished { get; }
    }

    /// <summary>
    /// Public IResult extensions for easier integrations
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Checks if any checkpoints have logged errors.
        /// </summary>
        /// <param name="result">reference result</param>
        public static bool HasErrors(this IResult result)
        {
            return result.Checkpoints.Any(r => r.Error != null);
        }

        /// <summary>
        /// Retrieves enumerable of checkpoints with errors
        /// </summary>
        /// <param name="result">reference result</param>
        public static IEnumerable<ICheckpoint> GetErrors(this IResult result)
        {
            var errors = result.Checkpoints
                .Where(c => c.Error != null);

            return errors;
        }
    }
}