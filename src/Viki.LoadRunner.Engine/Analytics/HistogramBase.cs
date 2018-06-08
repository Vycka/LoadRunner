using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Result;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Analytics.Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Utils;

namespace Viki.LoadRunner.Engine.Analytics
{
    public class Histogram<T> : HistogramBase<T, Histogram<T>>
    {

    }
    // TODO: Add's should have formatters, e.g. .Add(metric, formatter);
    // Parallelism can be achieved by making a processing chain
    // -> Enrich with Dim keys -> duplicate queues for each metric and feed them
    // because Parallel.Foreach(metrics,...) on each received cause really bad TPL overhead.
    public abstract class HistogramBase<TData, THistogram> : IHistogramBuilder<TData, THistogram>
    {
        #region Fields

        private FlexiRow<DimensionKey, IMetric<TData>> _row;

        private DimensionsHandler<TData> _dimensionsKeyBuilder;
        private MetricsHandler<TData> _metricMultiplexer;

        #endregion

        #region IHistogramBuilder

        public List<IDimension<TData>> Dimensions { get; } = new List<IDimension<TData>>();
        public List<IMetric<TData>> Metrics { get; } = new List<IMetric<TData>>();

        public Dictionary<string, string> ColumnAliases { get; } = new Dictionary<string, string>();
        public List<string> ColumnIgnoreNames { get; } = new List<string>();

        #endregion

        #region Aggregation methods

        public void Begin()
        {
            _metricMultiplexer = new MetricsHandler<TData>(Metrics);
            _row = new FlexiRow<DimensionKey, IMetric<TData>>(() => ((IMetric<TData>)_metricMultiplexer).CreateNew());

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
        /// Builds results into object having collumn names array and 2d array data grid
        /// </summary>
        public HistogramResults BuildResults()
        {

            OrderLearner orderLearner = new OrderLearner();
            _row.ForEach(i => orderLearner.Learn(i.Value.ColumnNames));

            string[] columnNames = Dimensions
                .Select(d => d.DimensionName)
                .Concat(orderLearner.LearnedOrder)
                .Except(ColumnIgnoreNames)
                .ToArray();

            object[][] resultValues = new object[_row.Count][];

            int rowIndex = 0;
            foreach (KeyValuePair<DimensionKey, IMetric<TData>> pair in _row)
            {
                resultValues[rowIndex] = new object[columnNames.Length];

                DimensionKey dimensions = pair.Key;
                IMetric<TData> metrics = pair.Value;

                for (int i = 0; i < dimensions.Values.Count; i++)
                {
                    resultValues[rowIndex][i] = dimensions.Values[i];
                }

                for (int i = 0; i < metrics.ColumnNames.Length; i++)
                {
                    int columnIndex = Array.FindIndex(columnNames, s => s == metrics.ColumnNames[i]);

                    if (columnIndex != -1)
                        resultValues[rowIndex][columnIndex] = metrics.Values[i];
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
            var results = BuildResults();

            foreach (var row in results.Values)
            {
                IDictionary<string, object> resultRow = new ExpandoObject();

                for (int j = 0; j < row.Length; j++)
                {
                    resultRow.Add(results.ColumnNames[j], row[j]);
                }

                yield return resultRow;
            }
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
