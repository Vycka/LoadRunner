using System;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Engine.Validators;
using Viki.LoadRunner.Playground.Replay;

namespace Viki.LoadRunner.Playground
{
    class Program
    {

        static void Main()
        {
            ReplayDemo.Run();

            new BlankFromBase().Validate();

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
