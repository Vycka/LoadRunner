using System;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Playground
{
    class Program
    {
        static void Main()
        {
            //ReadmeDemo.Run();

            var res = JsonStream.DeserializeSequenceFromJson<ReplayResult<Exception>>("d:\\test.stream.json")
                .Select(r => (IResult)r).ToList();

            HistogramAggregator histo = new HistogramAggregator();
            histo
                .Add(new TimeDimension(TimeSpan.FromSeconds(1)))
                .Add(new CountMetric())
                .Add(new AvgDurationMetric())
                .Add(new PercentileMetric(0.95));


            StreamAggregator.Replay(res, histo);
        }
    }
}
