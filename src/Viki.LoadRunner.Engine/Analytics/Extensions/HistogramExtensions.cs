using System.Collections.Generic;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics.Extensions
{
    public static class HistogramExtensions
    {
        /// <summary>
        /// Copies histogram setup from source to target, overwriting all preexisting configuration in target
        /// </summary>
        public static void CopySettings<TData, THistogram>(this IHistogramBuilder<TData, THistogram> source, IHistogramBuilder<TData, THistogram> target)
        {
            target.Dimensions.Clear();
            target.Dimensions.AddRange(source.Dimensions);

            target.Metrics.Clear();
            target.Metrics.AddRange(source.Metrics);

            target.ColumnAliases.Clear();
            foreach (KeyValuePair<string, string> item in source.ColumnAliases)
            {
                target.ColumnAliases.Add(item.Key, item.Value);
            }

            target.ColumnIgnoreNames.Clear();
            target.ColumnIgnoreNames.AddRange(source.ColumnIgnoreNames);
        }
    }
}