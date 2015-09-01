using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregates
{
    public interface IResultsAggregator
    {
        /// <summary>
        /// Results from all running threads will be poured into this one.
        /// </summary>
        void TestContextResultReceived(TestContextResult result);
    }
}