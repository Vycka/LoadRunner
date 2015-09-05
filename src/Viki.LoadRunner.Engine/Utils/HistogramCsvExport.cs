using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Results;
using Viki.LoadRunner.Engine.Aggregators.Utils;

namespace Viki.LoadRunner.Engine.Utils
{
    public static class HistogramCsvExport
    {
        public static void Export(IEnumerable<HistogramResultRow> results, string csvFilePath)
        {
            if (csvFilePath == null) throw new ArgumentNullException(nameof(csvFilePath));

            DataTable mappedHistgram = MapResultToTable(results);
            ExportCsv(mappedHistgram, csvFilePath);
        }

        #region Histogram results mapping

        private static DataTable MapResultToTable(IEnumerable<HistogramResultRow> results)
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("TimeStamp");

            foreach (HistogramResultRow histogramResultRow in results)
            {
                AddMissingResultItemRowColumns(dataTable, histogramResultRow.ResultItems);
                DataRow row = dataTable.NewRow();

                row["TimeStamp"] = histogramResultRow.TimePoint.ToUnixTime();

                AddResultItemRowValues(row, histogramResultRow.ResultItems);

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        private static void AddMissingResultItemRowColumns(DataTable dataTable,
            IEnumerable<ResultItemRow> resultItemRows)
        {
            foreach (ResultItemRow resultItemRow in resultItemRows)
            {
                AddColumnIfNeeded(dataTable, resultItemRow.Name, "MomentMin");
                AddColumnIfNeeded(dataTable, resultItemRow.Name, "MomentMax");
                AddColumnIfNeeded(dataTable, resultItemRow.Name, "MomentAverage");

                AddColumnIfNeeded(dataTable, resultItemRow.Name, "SummedMin");
                AddColumnIfNeeded(dataTable, resultItemRow.Name, "SummedMax");
                AddColumnIfNeeded(dataTable, resultItemRow.Name, "SummedAverage");

                AddColumnIfNeeded(dataTable, resultItemRow.Name, "SuccessIterationsPerSec");
                AddColumnIfNeeded(dataTable, resultItemRow.Name, "ErrorRatio");
                AddColumnIfNeeded(dataTable, resultItemRow.Name, "Count");
                AddColumnIfNeeded(dataTable, resultItemRow.Name, "ErrorCount");
            }
        }

        private static void AddColumnIfNeeded(DataTable dataTable, string resultItemName, string valueName)
        {
            string columnName = BuildColumnName(resultItemName, valueName);

            if (!dataTable.Columns.Contains(columnName))
                dataTable.Columns.Add(columnName);
        }

        private static void AddResultItemRowValues(DataRow row, IEnumerable<ResultItemRow> resultItemRows)
        {
            foreach (ResultItemRow resultRow in resultItemRows)
            {
                row[BuildColumnName(resultRow.Name, "MomentMin")] = (long)resultRow.MomentMin.TotalMilliseconds;
                row[BuildColumnName(resultRow.Name, "MomentMax")] = (long)resultRow.MomentMax.TotalMilliseconds;
                row[BuildColumnName(resultRow.Name, "MomentAverage")] = (long)resultRow.MomentAverage.TotalMilliseconds;

                row[BuildColumnName(resultRow.Name, "SummedMin")] = (long)resultRow.SummedMin.TotalMilliseconds;
                row[BuildColumnName(resultRow.Name, "SummedMax")] = (long)resultRow.SummedMax.TotalMilliseconds;
                row[BuildColumnName(resultRow.Name, "SummedAverage")] = (long)resultRow.SummedAverage.TotalMilliseconds;

                row[BuildColumnName(resultRow.Name, "SuccessIterationsPerSec")] = resultRow.SuccessIterationsPerSec;
                row[BuildColumnName(resultRow.Name, "ErrorRatio")] = resultRow.ErrorRatio;
                row[BuildColumnName(resultRow.Name, "Count")] = resultRow.Count;
                row[BuildColumnName(resultRow.Name, "ErrorCount")] = resultRow.ErrorCount;
            }
        }

        private static string BuildColumnName(string resultItemName, string valueName)
        {
            return String.Concat(resultItemName, " - ", valueName);
        }

        #endregion

        #region DataTable to csv export

        private static void ExportCsv(DataTable dataTable, string csvFilePath)
        {
            using (FileStream fileStream = new FileStream(csvFilePath, FileMode.Create))
            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                IEnumerable<string> columnNames = dataTable.Columns.Cast<DataColumn>().Select(column => string.Concat("\"", column.ColumnName.Replace("\"", "\"\""), "\""));
                streamWriter.WriteLine(string.Join(",", columnNames));

                foreach (DataRow row in dataTable.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));

                    streamWriter.WriteLine(string.Join(",", fields));
                }
            }
        }

        #endregion

    }
}