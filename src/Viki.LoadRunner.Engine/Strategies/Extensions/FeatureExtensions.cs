using System;
using System.Linq;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Extensions
{
    public static class FeatureExtensions
    {
        /// <summary>
        /// Sets aggregators to use when collecting data
        /// </summary>
        /// <param name="aggregagors">aggregators</param>
        /// <returns></returns>
        public static TStrategyBuilder SetAggregator<TStrategyBuilder>(this TStrategyBuilder builder, params IAggregator[] aggregagors)
            where TStrategyBuilder : IAggregatorFeature
        {
            builder.Aggregators = aggregagors;
            return builder;
        }

        /// <summary>
        /// Adds aggregators to use when collecting data
        /// </summary>
        /// <param name="aggregagors">aggregators</param>
        /// <returns></returns>
        public static TStrategyBuilder AddAggregator<TStrategyBuilder>(this TStrategyBuilder builder, params IAggregator[] aggregagors)
            where TStrategyBuilder : IAggregatorFeature
        {
            builder.Aggregators = builder.Aggregators.Concat(aggregagors).ToArray();
            return builder;
        }


        /// <summary>
        /// Sets timeout time for scenario execution threads to gracefully stop.
        /// </summary>
        /// <param name="timeout">timeout duration</param>
        public static TStrategyBuilder SetFinishTimeout<TStrategyBuilder>(this TStrategyBuilder builder, TimeSpan timeout)
            where TStrategyBuilder : ITimeoutFeature
        {
            builder.FinishTimeout = timeout;
            return builder;
        }

        /// <summary>
        /// Sets initial user data which will be passed to created thread contexts.
        /// </summary>
        /// <param name="userData">User-data object</param>
        public static TStrategyBuilder SetUserData<TStrategyBuilder>(this TStrategyBuilder builder, object userData)
            where TStrategyBuilder : IUserDataFeature
        {
            builder.InitialUserData = userData;
            return builder;
        }
    }
}