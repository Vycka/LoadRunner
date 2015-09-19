using System;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators
{
    public interface IResultsAggregator
    {
        /// <summary>
        /// Results from all running threads will be poured into this one.
        /// Avoid throwing exceptions outside, unless you want to stop the test
        /// </summary>
        void TestContextResultReceived(TestContextResult result);

        /// <summary>
        /// Signals aggregator, that new test execution is about to begin
        /// So aggregator can reset stats if needed.
        /// </summary>
        void Begin(DateTime testBeginTime);

        /// <summary>
        /// Signals aggregator, that new test execution is ended and stats aggregation is finished
        /// </summary>
        void End();
    }
}