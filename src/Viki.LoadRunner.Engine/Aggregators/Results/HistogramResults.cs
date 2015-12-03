using System;

namespace Viki.LoadRunner.Engine.Aggregators.Results
{
    public class HistogramResults
    {
        public readonly string[] ColumnNames;
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