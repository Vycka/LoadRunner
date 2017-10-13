using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Executor.Context.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Result
{

    /// <summary>
    /// IResult defines meassurement data structure
    /// this can be seen as fundamental raw data.
    /// Only the list of these IResult's are needed for meassured data aggregation.
    /// </summary>
    public interface IResult : IIterationResult 
    {
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