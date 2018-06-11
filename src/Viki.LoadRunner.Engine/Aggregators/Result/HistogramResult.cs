using System;
using System.Collections;
using System.Collections.Generic;

namespace Viki.LoadRunner.Engine.Aggregators.Result
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
        public readonly object[][] Data;

        public HistogramResults(string[] columnNames, object[][] data)
        {
            if (columnNames == null)
                throw new ArgumentNullException(nameof(columnNames));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            ColumnNames = columnNames;
            Data = data;
        }

        public HistogramResults Rows<T>(string columnName, T searchValue)
        {
            int keyIndex = FindColumn(columnName);
            if (keyIndex == -1)
                throw new ArgumentException("Bad columnName");

            List<object[]> filtered = new List<object[]>();

            for (int i = 0; i < Data.Length; i++)
            {
                if (((T)Data[i][keyIndex]).Equals(searchValue))
                    filtered.Add(Data[i]);
            }

            HistogramResults result = new HistogramResults(ColumnNames, filtered.ToArray());
            return result;
        }

        public IEnumerable<T> Values<T>(string columnName)
        {
            int keyIndex = FindColumn(columnName);
            if (keyIndex == -1)
                throw new ArgumentException("Bad columnName");

            foreach (var row in Data)
            {
                yield return (T)row[keyIndex];
            }
        }

        private int FindColumn(string key)
        {
            for (int i = 0; i < ColumnNames.Length; i++)
            {
                if (ColumnNames[i].Equals(key, StringComparison.InvariantCulture))
                    return i;
            }

            return -1;
        }
    }
}