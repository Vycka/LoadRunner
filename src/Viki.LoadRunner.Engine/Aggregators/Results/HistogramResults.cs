using System;

namespace Viki.LoadRunner.Engine.Aggregators.Results
{
    /// <summary>
    /// HistogramAggregator results class
    /// Results represented with ColumnNames[colIndex] object[colIndex][rows] arrays
    /// </summary>
    public class HistogramResults
    {
        /// <summary>
        /// Column headers
        /// </summary>
        public readonly string[] ColumnNames;

        /// <summary>
        /// 2d-array results
        /// </summary>
        public readonly object[][] Values;

        public HistogramResults(string[] columnNames, object[][] values)
        {
            if (columnNames == null)
                throw new ArgumentNullException(nameof(columnNames));
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            ColumnNames = columnNames;
            Values = values;
        }
    }
}