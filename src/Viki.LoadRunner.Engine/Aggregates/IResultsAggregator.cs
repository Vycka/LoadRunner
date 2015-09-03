using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregates
{
    public interface IResultsAggregator
    {
        /// <summary>
        /// Results from all running threads will be poured into this one.
        /// </summary>
        void TestContextResultReceived(TestContextResult result);

        /// <summary>
        /// Signals aggregator, that new test execution is about to begin
        /// So aggregator can reset stats if needed.
        /// </summary>
        void Reset();
    }
}