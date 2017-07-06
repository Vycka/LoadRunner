using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Aggregators.Results;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Result;
using Viki.LoadRunner.Engine.Utils;

namespace Viki.LoadRunner.Engine.Aggregators
{
    // TODO: Add's should have formatters, e.g. .Add(metric, formatter);
    // Parallelism can be achieved by making a processing chain
    // -> Enrich with Dim keys -> duplicate queues for each metric and feed them
    // because Parallel.Foreach(metrics,...) on each received cause really bad TPL overhead.

    /// <summary>
    /// Modular 2D grid histogram aggregator/builder. Use Add() method to register concrete IDiminension's and IMetric's
    /// </summary>
    public class HistogramAggregator : IResultsAggregator
    {
        #region Fields

        private FlexiRow<DimensionValues, IMetric> _row;

        private DimensionsKeyBuilder _dimensionsKeyBuilder;
        private MetricMultiplexer _metricMultiplexer;

        private readonly List<IDimension> _dimensions = new List<IDimension>();
        private readonly List<IMetric> _metricTemplates = new List<IMetric>();

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
        public HistogramAggregator Add(IDimension dimension)
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
        public HistogramAggregator Add(IMetric metric)
        {
            _metricTemplates.Add(metric);

            return this;
        }
    
        /// <summary>
        /// Ignore column when building results
        /// </summary>
        /// <param name="columnName">Column name to ignore</param>
        /// <returns>Current HistogramAggregator instance</returns>
        public HistogramAggregator Ignore(string columnName)
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
        public HistogramAggregator Alias(string sourceColumnName, string alias)
        {
            _columnNameAliases.Add(sourceColumnName, alias);

            return this;
        }

        #endregion

        #region IResultsAggregator

        void IResultsAggregator.Begin()
        {
            _metricMultiplexer = new MetricMultiplexer(_metricTemplates);
            _row = new FlexiRow<DimensionValues, IMetric>(() => ((IMetric)_metricMultiplexer).CreateNew());

            _dimensionsKeyBuilder = new DimensionsKeyBuilder(_dimensions);
        }

        void IResultsAggregator.TestContextResultReceived(IResult result)
        {
            DimensionValues key = _dimensionsKeyBuilder.GetValue(result);
            _row[key].Add(result);
        }

        void IResultsAggregator.End()
        {
        }

        #endregion

        #region Results builder

        /// <summary>
        /// Builds results into object having collumn names array and 2d array data grid
        /// </summary>
        public HistogramResults BuildResults()
        {
            if (_row == null)
                throw new InvalidOperationException("LoadTest wasn't performed with this HistogramAggregator");

            OrderLearner orderLearner = new OrderLearner();
            _row.ForEach(i => orderLearner.Learn(i.Value.ColumnNames));

            string[] columnNames = _dimensions
                .Select(d => d.DimensionName)
                .Concat(orderLearner.LearnedOrder)
                .Except(_ignoredColumnNames)
                .ToArray();

            object[][] resultValues = new object[_row.Count][];

            int rowIndex = 0;
            foreach (KeyValuePair<DimensionValues, IMetric> pair in _row)
            {
                resultValues[rowIndex] = new object[columnNames.Length];

                DimensionValues dimensions = pair.Key;
                IMetric metrics = pair.Value;

                for (int i = 0; i < dimensions.Values.Count; i++)
                {
                    resultValues[rowIndex][i] = dimensions.Values[i];
                }

                for (int i = 0; i < metrics.ColumnNames.Length; i++)
                {
                    int columnIndex = Array.FindIndex(columnNames, s => s == metrics.ColumnNames[i]);

                    if(columnIndex != -1)
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