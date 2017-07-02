using Viki.LoadRunner.Engine.Executor.Result;

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