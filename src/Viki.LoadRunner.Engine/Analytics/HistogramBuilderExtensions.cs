using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics
{
    public static class HistogramBuilderExtensions
    {
        /// <summary>
        /// Register dimension (aka X value)
        /// Each registered dimension will become part of composite key for aggregation.
        /// Think of each dimension as part of GROUP BY key.
        /// </summary>
        /// <param name="builder">Current histogram builder</param>
        /// <param name="dimension">dimension object</param>
        /// <returns>Current Histogram instance</returns>
        public static TBuilder Add<TData, TBuilder>(this IHistogramBuilder<TData, TBuilder> builder, IDimension<TData> dimension)
            where TBuilder : IHistogramBuilder<TData, TBuilder>
        {
            builder.Dimensions.Add(dimension);

            return (TBuilder)builder;
        }

        /// <summary>
        /// Register metric (aka Y value)
        /// Rows grouped by provided dimensions will be aggregated with registered metrics.
        /// </summary>
        /// <param name="builder">Current histogram builder</param>
        /// <param name="metric">metric object</param>
        /// <returns>Current Histogram instance</returns>
        public static TBuilder Add<TData, TBuilder>(this IHistogramBuilder<TData, TBuilder> builder, IMetric<TData> metric)
            where TBuilder : IHistogramBuilder<TData, TBuilder>
        {
            builder.Metrics.Add(metric);

            return (TBuilder)builder;
        }

        /// <summary>
        /// Ignore column when building results
        /// </summary>
        /// <param name="builder">Current histogram builder</param>
        /// <param name="columnName">Column name to ignore</param>
        /// <returns>Current Histogram instance</returns>
        public static TBuilder Ignore<TData, TBuilder>(this IHistogramBuilder<TData, TBuilder> builder, string columnName)
            where TBuilder : IHistogramBuilder<TData, TBuilder>
        {
            builder.ColumnIgnoreNames.Add(columnName);

            return (TBuilder)builder;
        }

        /// <summary>
        /// Rename result columns to desired names
        /// </summary>
        /// <param name="builder">Current histogram builder</param>
        /// <param name="sourceColumnName">source column name</param>
        /// <param name="alias">Name to replace it with</param>
        /// <returns>Current Histogram instance</returns>
        public static TBuilder Alias<TData, TBuilder>(this IHistogramBuilder<TData, TBuilder> builder, string sourceColumnName, string alias)
            where TBuilder : IHistogramBuilder<TData, TBuilder>
        {
            builder.ColumnAliases.Add(sourceColumnName, alias);

            return (TBuilder)builder;
        }
    }
}