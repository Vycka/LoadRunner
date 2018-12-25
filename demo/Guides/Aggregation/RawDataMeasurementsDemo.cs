using System;
using System.Linq;
using LoadRunner.Demo.Common;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Engine.Strategies.Extensions;
using Viki.LoadRunner.Tools.Aggregators;

namespace LoadRunner.Demo.Guides.Aggregation
{
    public static class RawDataMeasurementsDemo
    {
        // All IAggregator implementations receive un-aggregated raw measurements (later raw-data) from workers.
        //  - (TODO: More info in IAggregator customization, But IAggregator ir pretty self explanatory anyway :))
        // Main idea behind Raw-Data is that one gets un-aggregated measurements in IResult form
        // and one can do anything with it, preferably post-load-test.
        //
        //
        // Having this unlocks few advantages:
        // * Save this raw-data somewhere(file on disk, array in memory) and do any aggregations after the test.
        //   - I personally persist raw-data from all of the tests I do,
        //     because one will never know what aggregation might be needed until one runs the test and sees initial results.
        //   - There is a RnD JsonStreamAggregator which saves to JSON array file. Though it can take up some space on disk as JSON has lots of overhead.
        // * Or one can write own custom specialized aggregation ignoring provided HistogramAggregator totally.
        //
        //
        // This demo contains:
        // * Load-test with one aggregation:
        //    - [Tools nuget: JsonStreamAggregator] Write data to raw.json file for later aggregation
        // * Replay of raw measurements after the test and do some more aggregating post-load-test.
        //    - Two histogram aggregators: Timeline and KPI.
        //    - One StreamAggregator instance which gives IEnumerable of IResult's for custom inline aggregation.

        public static void Run()
        {
            // JsonStreamAggregator dumps everything it receives to file.
            JsonStreamAggregator jsonAggregator = new JsonStreamAggregator("raw.json");
            

            StrategyBuilder builder = new StrategyBuilder()
                .AddLimit(new TimeLimit(TimeSpan.FromSeconds(4)))
                .SetThreading(new FixedThreadCount(4))
                .SetScenario(new SleepingScenarioFactory(TimeSpan.FromMilliseconds(100))) // Use scenario from common which sleeps every iteration
                .SetAggregator(jsonAggregator); // Register JsonAggregator to be only one to receive results

            // A quick run
            builder.Build().Run();


            // Now lets do actual aggregation post testing.
            // First - just configure HistogramAggregator or any other aggregator in same way one would do for the live test.

            HistogramAggregator histogramTimeline = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(1)))
                .Add(new CountMetric())
                .Add(new GlobalTimerMinValueMetric())
                .Add(new GlobalTimerMaxValueMetric())
                .Add(new GlobalTimerPeriodMetric());

            HistogramAggregator histogramKpi = new HistogramAggregator()
                .Add(new CountMetric())
                .Add(new GlobalTimerMinValueMetric())
                .Add(new GlobalTimerMaxValueMetric())
                .Add(new GlobalTimerPeriodMetric());

            // Lets define another more specialized aggregation just for lols.
            TimeSpan lastIterationEnded = TimeSpan.Zero;
            StreamAggregator streamAggregator = new StreamAggregator(
                results =>
                {
                    lastIterationEnded = results.Max(r => r.IterationFinished);
                }
            );

            // now just replay saved raw-data stream to those later-defined aggregations.
            JsonStreamAggregator.Replay("raw.json", histogramTimeline, histogramKpi, streamAggregator);

            Console.WriteLine($"StreamAggregator, last iteration  finished: {lastIterationEnded:g}");
            Console.WriteLine("---------------- Histogram KPI -----------");
            Console.WriteLine(JsonConvert.SerializeObject(histogramKpi.BuildResultsObjects(), Formatting.Indented));
            Console.WriteLine("---------------- Histogram timeline -----------");
            Console.WriteLine(JsonConvert.SerializeObject(histogramTimeline.BuildResultsObjects(), Formatting.Indented));
        }
    }
}