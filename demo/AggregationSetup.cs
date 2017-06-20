using System;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Tools.Aggregators;

namespace LoadRunner.Demo
{
    public static class AggregationSetup
    {
        private static readonly string[] IgnoredCheckpoints =
{
                Checkpoint.IterationSetupCheckpointName,
                Checkpoint.IterationStartCheckpointName,
                Checkpoint.IterationTearDownCheckpointName
        };

        public static HistogramAggregator BuildHistogram()
        {
            // This demo shows this new HistogramAggregator which isn't currently documented anywhere else. (TODO)
            //
            // This preset shown below more or less should work for most of the test cases, except for TimeDimension,
            // which you might want to adjust for bigger chunks or remove it to get totals [no dimensions == no grouping by].
            //
            // HistogramAggregator is a modular Dimension/Metric style aggregator tool, which can be expanded by implementing IMetric or IDimension
            //
            // Dimensions are like row keys (Adding multiple dimensions would multiply result row count)
            // * Currently only TimeDimension and FuncDimension is available
            // Metrics are like values, which will be meassured in test execution
            // * Most usable metrics are all defined below
            HistogramAggregator histogramAggregator = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(10)))
                .Add(new MinDurationMetric(IgnoredCheckpoints))
                .Add(new AvgDurationMetric(IgnoredCheckpoints))
                .Add(new MaxDurationMetric(IgnoredCheckpoints))
                .Add(new PercentileMetric(new[] { 0.5, 0.8, 0.9, 0.95, 0.99 }, IgnoredCheckpoints))
                .Add(new CountMetric(IgnoredCheckpoints))
                .Add(new ErrorCountMetric())
                .Add(new FuncMetric<int>("Created Threads", 0, (i, result) => result.CreatedThreads))
                .Alias($"Min: {Checkpoint.IterationEndCheckpointName}", "Min (ms)")
                .Alias($"Avg: {Checkpoint.IterationEndCheckpointName}", "Avg (ms)")
                .Alias($"Max: {Checkpoint.IterationEndCheckpointName}", "Max (ms)")
                .Alias($"50%: {Checkpoint.IterationEndCheckpointName}", "50% (ms)")
                .Alias($"80%: {Checkpoint.IterationEndCheckpointName}", "80% (ms)")
                .Alias($"90%: {Checkpoint.IterationEndCheckpointName}", "90% (ms)")
                .Alias($"95%: {Checkpoint.IterationEndCheckpointName}", "95% (ms)")
                .Alias($"99%: {Checkpoint.IterationEndCheckpointName}", "99% (ms)")
                .Alias($"Count: {Checkpoint.IterationEndCheckpointName}", "Success: Count")
                .Alias($"Errors: {Checkpoint.IterationSetupCheckpointName}", "Errors: Setup")
                .Alias($"Errors: {Checkpoint.IterationStartCheckpointName}", "Errors: Iteration")
                .Alias($"Errors: {Checkpoint.IterationTearDownCheckpointName}", "Errors: Teardown");

            return histogramAggregator;
        }


        // Advanced optional at first, but useful when going to PROD.
        // This allows to do post-test aggregations in any slices you will think later.
        //
        // Checkout RawDataAggregationDemo.cs if interested.
        public static IResultsAggregator BuildJsonStreamAggregator()
        {
            return new JsonStreamAggregator("masterdata.json");
        }
    }
}
