using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Aggregators.Results;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Utils;

namespace Viki.LoadRunner.Engine.Aggregators
{
    
    /// <summary>
    /// Generic X/Y histogram aggregator/builder. Use RegisterDimension() and AddMetric() methods to add concrete aggregates
    /// </summary>
    public class HistogramAggregator : IResultsAggregator
    {
        #region Fields

        private FlexiGrid<DimensionValues, IMetric> _grid;

        private DimensionsKeyBuilder _dimensionsKeyBuilder;
        private readonly List<IDimension> _dimensions = new List<IDimension>();

        private readonly List<IMetric> _metricTemplates = new List<IMetric>();
        private MetricMultiplexer _metricMultiplexer;

        private readonly Dictionary<string, string> _columnNameAliases = new Dictionary<string, string>(); 
        #endregion

        #region Construction

        /// <summary>
        /// Register dimension (aka X value)
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
        /// </summary>
        /// <returns>Current HistogramAggregator instance</returns>
        public HistogramAggregator Add(IMetric metricTemplate)
        {
            _metricTemplates.Add(metricTemplate);

            return this;
        }

        /// <summary>
        /// Rename result columns to desired names
        /// </summary>
        /// <param name="sourceColumnName">source column name</param>
        /// <param name="alias">Name to replace it with</param>
        public HistogramAggregator Alias(string sourceColumnName, string alias)
        {
            _columnNameAliases.Add(sourceColumnName, alias);

            return this;
        }

        #endregion

        #region IResultsAggregator

        void IResultsAggregator.TestContextResultReceived(TestContextResult result)
        {
            DimensionValues key = _dimensionsKeyBuilder.GetValue(result);
            _grid[key].Add(result);
        }

        void IResultsAggregator.Begin(DateTime testBeginTime)
        {
            _metricMultiplexer = new MetricMultiplexer(_metricTemplates);
            _grid = new FlexiGrid<DimensionValues, IMetric>((() => _metricMultiplexer.CreateNew()));

            _dimensionsKeyBuilder = new DimensionsKeyBuilder(_dimensions);
        }

        void IResultsAggregator.End()
        {
        }

        #endregion

        #region Results

        /// <summary>
        /// Builds results results into object having collumn names array  2d array data grid
        /// </summary>
        public HistogramResults BuildResults()
        {
            OrderLearner orderLearner = new OrderLearner();
            _grid.ForEach(i => orderLearner.Learn(i.Value.ColumnNames));

            string[] columnNames = _dimensions
                .Select(d => d.DimensionName)
                .Concat(orderLearner.LearnedOrder)
                .ToArray();

            object[][] resultValues = new object[_grid.Count][];

            int rowIndex = 0;
            foreach (KeyValuePair<DimensionValues, IMetric> pair in _grid)
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
        public IEnumerable<dynamic> BuildResultsDynamic()
        {
            var results = BuildResults();

            for (int i = 0; i < results.Values.Length; i++)
            {
                var row = results.Values[i];
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