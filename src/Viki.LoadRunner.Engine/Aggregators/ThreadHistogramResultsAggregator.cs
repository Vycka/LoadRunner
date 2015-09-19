namespace Viki.LoadRunner.Engine.Aggregators
{
    public class ThreadHistogramResultsAggregator : HistogramResultsAggregator
    {

        public ThreadHistogramResultsAggregator()
            : base(result => result.ThreadId)
        {
        }
    }
}