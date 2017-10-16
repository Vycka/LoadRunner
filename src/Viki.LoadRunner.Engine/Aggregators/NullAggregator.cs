using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Executor.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators
{
    public class NullAggregator : IResultsAggregator
    {
        public void Begin()
        {
        }

        public void TestContextResultReceived(IResult result)
        {
        }

        public void End()
        {
        }
    }
}