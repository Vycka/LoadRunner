using System;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Tools.Aggregators;

namespace Viki.LoadRunner.Playground
{
    class Program
    {
        static void Main()
        {
            HistogramAggregator histo = new HistogramAggregator();
            histo
                .Add(new TimeDimension(TimeSpan.FromSeconds(1)))
                .Add(new CountMetric())
                .Add(new AvgDurationMetric())
                .Add(new PercentileMetric(0.95));


            JsonStreamAggregator.Replay("d:\\test.stream.json", histo);
        }
    }
}
