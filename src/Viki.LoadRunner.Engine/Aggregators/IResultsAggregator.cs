using System;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Engine.Aggregators
{
    public interface IResultsAggregator
    {
        /// <summary>
        /// Results from all running threads will be poured into this one.
        /// Avoid throwing exceptions outside, unless you want to stop the test
        /// </summary>
        void TestContextResultReceived(IResult result);

        /// <summary>
        /// Signals aggregator, that new test execution is about to begin
        /// So aggregator can reset stats if needed.
        /// </summary>
        void Begin();

        /// <summary>
        /// Signals aggregator, that new test execution is ended and stats aggregation is finished
        /// </summary>
        void End();
    }
}