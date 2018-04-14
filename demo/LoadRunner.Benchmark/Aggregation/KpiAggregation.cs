using System;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Tools.ConsoleUi;

namespace LoadRunner.Benchmark.Aggregation
{
    public class KpiAggregation
    {
        public static IAggregator Build()
        {
            return new KpiOutput(TimeSpan.FromSeconds(3));
            // TODO FINISH BuiLDER;

        }
    }
}