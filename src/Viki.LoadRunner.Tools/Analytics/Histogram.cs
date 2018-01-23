using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Result;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Utils;
using Viki.LoadRunner.Tools.Analytics.Viki.LoadRunner.Engine.Aggregators.Utils;

namespace Viki.LoadRunner.Tools.Analytics
{
    class Histogram<T>
    {
        #region Fields

        private FlexiRow<DimensionKey, IMetric<T>> _row;

        private DimensionsGroup<T> _dimensionsKeyBuilder;
        private MetricsHandler<T> _metricMultiplexer;

        private readonly List<IDimension<T>> _dimensions = new List<IDimension<T>>();
        private readonly List<IMetric<T>> _metricTemplates = new List<IMetric<T>>();

        private readonly Dictionary<string, string> _columnNameAliases = new Dictionary<string, string>();
        private readonly List<string> _ignoredColumnNames = new List<string>();

        #endregion

        #region Construction

        /// <summary>
        /// Register dimension (aka X value)
        /// Each registered dimension will become part of composite key for aggregation.
        /// Think of each dimension as part of GROUP BY key.
        /// </summary>
        /// <param name="dimension">dimension object</param>
        /// <returns>Current HistogramAggregator instance</returns>
        public Histogram<T> Add(IDimension<T> dimension)
        {
            _dimensions.Add(dimension);

            return this;
        }

        /// <summary>
        /// Register metric (aka Y value)
        /// Rows grouped by provided dimensions will be aggregated with registered metrics.
        /// </summary>
        /// <param name="metric">metric object</param>
        /// <returns>Current HistogramAggregator instance</returns>
        public Histogram<T> Add(IMetric<T> metric)
        {
            _metricTemplates.Add(metric);

            return this;
        }

        /// <summary>
        /// Ignore column when building results
        /// </summary>
        /// <param name="columnName">Column name to ignore</param>
        /// <returns>Current HistogramAggregator instance</returns>
        public Histogram<T> Ignore(string columnName)
        {
            _ignoredColumnNames.Add(columnName);

            return this;
        }

        /// <summary>
        /// Rename result columns to desired names
        /// </summary>
        /// <param name="sourceColumnName">source column name</param>
        /// <param name="alias">Name to replace it with</param>
        /// <returns>Current HistogramAggregator instance</returns>
        public Histogram<T> Alias(string sourceColumnName, string alias)
        {
            _columnNameAliases.Add(sourceColumnName, alias);

            return this;
        }

        #endregion

        #region IResultsAggregator

        public void Begin()
        {
            _metricMultiplexer = new MetricsHandler<T>(_metricTemplates);
            _row = new FlexiRow<DimensionKey, IMetric<T>>(() => ((IMetric<T>)_metricMultiplexer).CreateNew());

            _dimensionsKeyBuilder = new DimensionsGroup<T>(_dimensions);
        }


        public void Aggregate(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                DimensionKey key = _dimensionsKeyBuilder.GetValue(item);
                _row[key].Add(item);
            }
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

            string[] columnNames = _dimensions
                .Select(d => d.DimensionName)
                .Concat(orderLearner.LearnedOrder)
                .Except(_ignoredColumnNames)
                .ToArray();

            object[][] resultValues = new object[_row.Count][];

            int rowIndex = 0;
            foreach (KeyValuePair<DimensionKey, IMetric<T>> pair in _row)
            {
                resultValues[rowIndex] = new object[columnNames.Length];

                DimensionKey dimensions = pair.Key;
                IMetric<T> metrics = pair.Value;

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

            ReplaceNames(columnNames, _columnNameAliases);

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
