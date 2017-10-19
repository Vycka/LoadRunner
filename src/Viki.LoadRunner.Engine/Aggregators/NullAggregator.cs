using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

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