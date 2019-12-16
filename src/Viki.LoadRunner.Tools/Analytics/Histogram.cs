using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Result;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Analytics.Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Utils;

namespace Viki.LoadRunner.Tools.Analytics
{
    public class Histogram<TData, THistogram>
    {
        #region Fields

        private FlexiRow<DimensionKey, MetricsHandler<TData>> _row;

        private DimensionsHandler<TData> _dimensionsKeyBuilder;
        private MetricsHandler<TData> _metricMultiplexer;

        #endregion

        #region IHistogramBuilder

        public List<IDimension<TData>> Dimensions { get; } = new List<IDimension<TData>>();
        public List<IMetric<TData>> Metrics { get; } = new List<IMetric<TData>>();

        public Dictionary<string, string> ColumnAliases { get; } = new Dictionary<string, string>();
        public List<string> ColumnIgnoreNames { get; } = new List<string>();

        public Dictionary<int, PostProcessDelegate> MetricsPostProcess { get; } = new Dictionary<int, PostProcessDelegate>();

        #endregion

        #region Aggregation methods

        public void Begin()
        {
            _metricMultiplexer = new MetricsHandler<TData>(Metrics, MetricsPostProcess);
            _row = new FlexiRow<DimensionKey, MetricsHandler<TData>>(() => _metricMultiplexer.Create());

            _dimensionsKeyBuilder = new DimensionsHandler<TData>(Dimensions);
        }

        public void Aggregate(IEnumerable<TData> items)
        {
            foreach (TData item in items)
            {
                Aggregate(item);
            }
        }

        public void Aggregate(TData item)
        {
            DimensionKey key = _dimensionsKeyBuilder.GetValue(item);
            _row[key].Add(item);
        }

        #endregion

        #region Results builder

        /// <summary>
        /// Builds results into object having column names array and 2d array data grid
        /// </summary>
        public HistogramResults BuildResults()
        {
            if (_row == null)
                throw new InvalidOperationException("Aggregation was never performed.");

            KeyValuePair<DimensionKey, Val[]>[] results = _row
                .Select(r => new KeyValuePair<DimensionKey, Val[]>(r.Key, r.Value.Export().ToArray()))
                .ToArray();

            OrderLearner orderLearner = new OrderLearner();
            results.ForEach(kv => orderLearner.Learn(kv.Value.Select(v => v.Key)));

            string[] columnNames = Dimensions
                .Select(d => d.DimensionName)
                .Concat(orderLearner.LearnedOrder)
                .Except(ColumnIgnoreNames)
                .ToArray();

            object[][] resultValues = new object[_row.Count][];

            int rowIndex = 0;
            foreach (KeyValuePair<DimensionKey, Val[]> pair in results)
            {
                resultValues[rowIndex] = new object[columnNames.Length];

                DimensionKey dimensions = pair.Key;
                Val[] metrics = pair.Value;

                for (int i = 0; i < dimensions.Values.Count; i++)
                {
                    resultValues[rowIndex][i] = dimensions.Values[i];
                }

                for (int i = 0; i < metrics.Length; i++)
                {
                    int columnIndex = Array.FindIndex(columnNames, s => s == metrics[i].Key);

                    if (columnIndex != -1)
                        resultValues[rowIndex][columnIndex] = metrics[i].Value;
                }

                rowIndex++;
            }

            ReplaceNames(columnNames, ColumnAliases);

            return new HistogramResults(columnNames, resultValues);
        }

        /// <summary>
        /// Builds dynamic results objects list, where each object has property name equal to column name.
        /// Result serialized to JSON it would produce output, which compatible with online JSON -> CSV converters.
        /// </summary>
        public IEnumerable<object> BuildResultsObjects()
        {
            return BuildResults().ToObjects();
        }

        private static void ReplaceNames(string[] data, Dictionary<string, string> replaceTable)
        {
            for (int i = 0; i < data.Length; i++)
            {
                string targetName;
                if (replaceTable.TryGetValue(data[i], out targetName))
                {
                    data[i] = targetName;
                }
            }
        }

        #endregion
    }
}