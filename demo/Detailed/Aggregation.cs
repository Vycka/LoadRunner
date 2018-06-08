using System;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Core.Scenario;

namespace LoadRunner.Demo.Detailed
{
    public static class Aggregation
    {
        private static readonly string[] IgnoredCheckpoints =
{
                Checkpoint.Names.Setup,
                Checkpoint.Names.IterationStart,
                Checkpoint.Names.TearDown
        };

        public static HistogramAggregator BuildHistogram()
        {
            // This demo shows this new HistogramAggregator which isn't currently documented anywhere else. (TODO)
            //
            // This preset shown below more or less should work for most of the test cases, except for TimeDimension,
            // which you might want to adjust for bigger chunks or remove it to get totals [no dimensions == no grouping by].
            //
            // HistogramAggregator is a modular Dimension/Metric style aggregator tool, which can be expanded by implementing IMetric or IDimension
            //  * Metrics are aggregation functions like COUNT, SUM, etc..
            //
            // Dimensions are like row keys (Adding multiple dimensions would multiply result row count)
            // * In SQL it would be like GROUP BY function
            // Available IDimension's:
            //   TimeDimension(TimeSpan interval, string dimensionName = "Time (s)")
            //   FuncDimension(string dimensionName, Func<IResult,string> dimensionValueSelector)
            //
            // Metrics are like values, which will be meassured in test execution
            // In SQL it would be like aggregation function, like SUM, COUNT, etc.
            // Available IMetric's:
            //   AvgDurationMetric(params string[] ignoredCheckpoints)
            //   BreakByMetric(IDimension subDimension, params IMetric[] actualMetrics)
            //   CountMetric(params string[] ignoredCheckpoints)
            //   ErrorCountMetric(bool includeTotals = true)
            //   ErrorRatioMetric(params string[] ignoredCheckpoints)
            //   FuncMetric(string keyName, TValue initialValue, Func<TValue, IResult, TValue> metricFunc) 
            //   FuncMultiMetric(Action<FlexiRow<string,TValue>, IResult> metricProcedure, Func<TValue> cellBuilderFunc)
            //   MaxDurationMetric(params string[] ignoredCheckpoints)
            //   MinDurationMetric(params string[] ignoredCheckpoints)
            //   PercentileMetric(double[] percentiles, string[] ignoredCheckpoints)
            //   TransactionsPerSecMetric()
            HistogramAggregator histogramAggregator = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(10)))
                .Add(new MinDurationMetric(IgnoredCheckpoints))
                .Add(new AvgDurationMetric(IgnoredCheckpoints))
                .Add(new MaxDurationMetric(IgnoredCheckpoints))
                .Add(new PercentileMetric(new[] { 0.95, 0.99 }, IgnoredCheckpoints))
                .Add(new CountMetric(IgnoredCheckpoints))
                .Add(new ErrorCountMetric())
                .Add(new FuncMetric<int>("Created Threads", 0, (i, result) => result.CreatedThreads))
                .Alias($"Min: {Checkpoint.Names.IterationEnd}", "Min (ms)")
                .Alias($"Avg: {Checkpoint.Names.IterationEnd}", "Avg (ms)")
                .Alias($"Max: {Checkpoint.Names.IterationEnd}", "Max (ms)")
                .Alias($"95%: {Checkpoint.Names.IterationEnd}", "95% (ms)")
                .Alias($"99%: {Checkpoint.Names.IterationEnd}", "99% (ms)")
                .Alias($"Count: {Checkpoint.Names.IterationEnd}", "Success: Count")
                .Alias($"Errors: {Checkpoint.Names.Setup}", "Errors: Setup")
                .Alias($"Errors: {Checkpoint.Names.IterationStart}", "Errors: Iteration")
                .Alias($"Errors: {Checkpoint.Names.TearDown}", "Errors: Teardown");

            return histogramAggregator;
        }
    }
}
