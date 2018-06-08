using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators
{
    /// <summary>
    /// Modular 2D grid histogram aggregator/builder. Use Add() method to register concrete IDiminension's and IMetric's
    /// </summary>
    public class HistogramAggregator : HistogramBase<IResult, HistogramAggregator>, IAggregator
    {
        public void End()
        {
        }
    }
}