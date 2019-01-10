using System;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Core.Scenario;

namespace LoadRunner.Demo.Guides.Aggregation
{
    public class HistogramAggregatorDemo
    {
        public static void Run()
        {
            // Proper demo is being written ATM.
            // So for time being below a common preset is provided.


            // HistogramAggregator is a modular Dimension/Metric style aggregator tool,
            // which can be expanded by implementing own custom IMetric or IDimension.
            // 
            // Dimensions are like row keys (Adding multiple dimensions would also multiply result row count)
            // * In SQL it would be like GROUP BY function, or GroupBy in Linq
            // * Dimensions are optional, which would result data aggregated into single KPI row.
            // Metrics are aggregation functions which will aggregate data for each slice presented by Dimensions
            // * In SQL it would be aggregation functions like COUNT, SUM, AVERAGE, etc..


            // In this preset data is grouped by 10 second periods.
            // One can change period below, or totally remove TimeDimension to get KPI result.
            HistogramAggregator histogramAggregator = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(10)))
                .Add(new MinDurationMetric())
                .Add(new AvgDurationMetric())
                .Add(new MaxDurationMetric())
                .Add(new PercentileMetric(new[] { 0.95, 0.99 }))
                .Add(new CountMetric(Checkpoint.NotMeassuredCheckpoints))
                .Add(new TransactionsPerSecMetric())
                .Add(new ErrorCountMetric())
                .Add(new FuncMetric<int>("Created Threads", 0, (i, result) => result.CreatedThreads))
                .Alias($"Min: {Checkpoint.Names.Iteration}", "Min (ms)")
                .Alias($"Avg: {Checkpoint.Names.Iteration}", "Avg (ms)")
                .Alias($"Max: {Checkpoint.Names.Iteration}", "Max (ms)")
                .Alias($"95%: {Checkpoint.Names.Iteration}", "95% (ms)")
                .Alias($"99%: {Checkpoint.Names.Iteration}", "99% (ms)")
                .Alias($"Count: {Checkpoint.Names.Iteration}", "Success: Count")
                .Alias($"Errors: {Checkpoint.Names.Setup}", "Errors: Setup")
                .Alias($"Errors: {Checkpoint.Names.Iteration}", "Errors: Iteration")
                .Alias($"Errors: {Checkpoint.Names.TearDown}", "Errors: Teardown");
        }
    }
}