using System;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Strategies.Threading;
using Viki.LoadRunner.Tools.Aggregators;

namespace Viki.LoadRunner.Playground
{
    class Program
    {

        static void Main()
        {

            DemoSetup.Run();
            

            return;

            HistogramAggregator histo = new HistogramAggregator();
            histo
                .Add(new TimeDimension(TimeSpan.FromSeconds(10)) { TimeSelector = i => i.IterationStarted })
                .Add(new IncrementalThreadCount(15, TimeSpan.FromSeconds(10), 15))
                .Add(new CountMetric())
                .Add(new AvgDurationMetric())
                .Add(new PercentileMetric(0.95) { Formatter =  l => l });


            //JsonStreamAggregator.Replay("17_13_22__1859.json", histo);

            Console.WriteLine(JsonConvert.SerializeObject(histo.BuildResultsObjects(), Formatting.Indented));
        }
    }
}
