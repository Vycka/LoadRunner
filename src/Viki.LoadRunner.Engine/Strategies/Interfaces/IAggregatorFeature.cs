using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Interfaces
{
    public interface IAggregatorFeature : IStrategyBuilder
    {
        /// <summary>
        /// Aggregators to collect the data.
        /// </summary>
        IAggregator[] Aggregators { get; set; }
    }
}