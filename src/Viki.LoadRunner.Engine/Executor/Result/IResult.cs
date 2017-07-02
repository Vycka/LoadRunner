using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Executor.Result
{

    /// <summary>
    /// IResult defines meassurement data structure
    /// this can be seen as fundamental raw data.
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
        /// Retrieves enumerable of errors in logged checkpoints.
        /// </summary>
        /// <param name="result">reference result</param>
        public static IEnumerable<Exception> GetErrors(this IResult result)
        {
            var errors = result.Checkpoints
                .Where(c => c.Error != null)
                .Select(c => c.Error);

            return errors;
        }
    }
}