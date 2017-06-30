using System;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Engine.Aggregators
{
    public interface IResultsAggregator
    {
        /// <summary>
        /// Signals aggregator, that new test execution is about to begin
        /// Aggregator can reset stats here if needed.
        /// </summary>
        void Begin();

        /// <summary>
        /// Results from all running threads will be poured into this one.
        /// </summary>
        /// <remarks>Exceptions are unhandled here and will break test execution.</remarks>
        void TestContextResultReceived(IResult result);

        /// <summary>
        /// Signals aggregator, that test execion has ended and all meassurements have been delivered.
        /// </summary>
        void End();
    }
}