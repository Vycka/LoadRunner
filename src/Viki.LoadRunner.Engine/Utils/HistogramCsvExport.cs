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
        public static int DoublePrecision = 3;

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

            dataTable.Columns.Add("Key");
            dataTable.Columns.Add("WorkingThreadsAvg");
            dataTable.Columns.Add("CreatedThreadsAvg");
            dataTable.Columns.Add("TotalErrors");

            foreach (HistogramResultRow histogramResultRow in results)
            {
                AddMissingResultItemRowColumns(dataTable, histogramResultRow.ResultItems);
                DataRow row = dataTable.NewRow();

                row["Key"] = histogramResultRow.GroupByKey;
                row["WorkingThreadsAvg"] = Math.Round(histogramResultRow.WorkingThreads, DoublePrecision);
                row["CreatedThreadsAvg"] = Math.Round(histogramResultRow.CreatedThreads, DoublePrecision);
                row["TotalErrors"] = histogramResultRow.ResultItems.Sum(r => r.ErrorCount);

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
                string resultItemName = resultItemRow.Name.Replace("SYS_ITERATION_", "SYS_IT_");

                if (resultItemName != "SYS_IT_SETUP")
                {
                    AddColumnIfNeeded(dataTable, resultItemName, "MomentMin");
                    AddColumnIfNeeded(dataTable, resultItemName, "MomentAvg");
                    AddColumnIfNeeded(dataTable, resultItemName, "MomentMax");

                    AddColumnIfNeeded(dataTable, resultItemName, "SummedMin");
                    AddColumnIfNeeded(dataTable, resultItemName, "SummedAvg");
                    AddColumnIfNeeded(dataTable, resultItemName, "SummedMax");
                }

                AddColumnIfNeeded(dataTable, resultItemName, "SuccessIterationsPerSec");
                AddColumnIfNeeded(dataTable, resultItemName, "ErrorRatio");
                AddColumnIfNeeded(dataTable, resultItemName, "Count");
                AddColumnIfNeeded(dataTable, resultItemName, "ErrorCount");
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
                string resultItemName = resultRow.Name.Replace("SYS_ITERATION_", "SYS_IT_");

                if (resultItemName != "SYS_IT_SETUP")
                {
                    row[BuildColumnName(resultItemName, "MomentMin")] = (long) resultRow.MomentMin.TotalMilliseconds;
                    row[BuildColumnName(resultItemName, "MomentMax")] = (long) resultRow.MomentMax.TotalMilliseconds;
                    row[BuildColumnName(resultItemName, "MomentAvg")] = (long) resultRow.MomentAvg.TotalMilliseconds;

                    row[BuildColumnName(resultItemName, "SummedMin")] = (long) resultRow.SummedMin.TotalMilliseconds;
                    row[BuildColumnName(resultItemName, "SummedMax")] = (long) resultRow.SummedMax.TotalMilliseconds;
                    row[BuildColumnName(resultItemName, "SummedAvg")] = (long) resultRow.SummedAvg.TotalMilliseconds;
                }
                row[BuildColumnName(resultItemName, "SuccessIterationsPerSec")] = Math.Round(resultRow.SuccessIterationsPerSec, DoublePrecision);
                row[BuildColumnName(resultItemName, "ErrorRatio")] = Math.Round(resultRow.ErrorRatio, DoublePrecision);
                row[BuildColumnName(resultItemName, "Count")] = resultRow.Count;
                row[BuildColumnName(resultItemName, "ErrorCount")] = resultRow.ErrorCount;
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