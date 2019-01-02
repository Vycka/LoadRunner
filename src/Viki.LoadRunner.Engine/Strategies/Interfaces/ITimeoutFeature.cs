using System;

namespace Viki.LoadRunner.Engine.Strategies.Interfaces
{
    public interface ITimeoutFeature : IStrategyBuilder
    {
        /// <summary>
        /// Timeout for scenario execution threads to gracefully stop.
        /// Failed to complete threads will get Thread.Abort()
        /// </summary>
        TimeSpan FinishTimeout { get; set; }
    }
}