using System;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Playground.Replay;

namespace Viki.LoadRunner.Playground
{
    class Program
    {

        static void Main()
        {
            BlankScenario.Run();

            //ReplayDemo.Run();

            //BatchStrategyDemo.Run();

            //new BlankFromBase().Validate();

            //DemoSetup.Run();

            //LimitConcurrencyAndTpsDemo.Run();

            // Warning! Hdd/sdd intensive usage with this one
            //BlankStressScenarioJsonStream.Run();

            Console.ReadKey();

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
