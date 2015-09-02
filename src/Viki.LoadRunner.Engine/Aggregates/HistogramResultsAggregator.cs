using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregates.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregates
{
    public class HistogramResultsAggregator : IResultsAggregator
    {
        private readonly int _aggregationStepSeconds;
        private readonly Dictionary<int, Dictionary<string, ResultItem>> _histogramItems = new Dictionary<int, Dictionary<string, ResultItem>>();

        private readonly static Checkpoint PreviousCheckpointBase = new Checkpoint("", TimeSpan.Zero);

        public HistogramResultsAggregator(int aggregationStepSeconds = 1)
        {
            _aggregationStepSeconds = aggregationStepSeconds;
        }

        public void TestContextResultReceived(TestContextResult result)
        {
            int histogramRowTimeSlot = GetHistogramRowTimeSlot(result.IterationFinished);
            Dictionary<string, ResultItem> histogramRow = GetHistogramRow(histogramRowTimeSlot);

            Checkpoint previousCheckpoint = PreviousCheckpointBase;
            foreach (Checkpoint currentCheckpoint in result.Checkpoints)
            {
                TimeSpan momentCheckpointTimeSpan = currentCheckpoint.TimePoint - previousCheckpoint.TimePoint;
                ResultItem checkpointResultObject = GetHistogramTargetResultObject(histogramRow, currentCheckpoint.CheckpointName);

                checkpointResultObject.AggregateResult(momentCheckpointTimeSpan, currentCheckpoint, result);
                previousCheckpoint = currentCheckpoint;
            }
        }

        private ResultItem GetHistogramTargetResultObject(Dictionary<string, ResultItem> histogramRow, string checkpointName)
        {
            ResultItem result = null;
            if (!histogramRow.TryGetValue(checkpointName, out result))
            {
                result = new ResultItem(checkpointName);
                histogramRow.Add(checkpointName, result);
            }

            return result;
        }

        private Dictionary<string, ResultItem> GetHistogramRow(int timeslot)
        {
            Dictionary<string, ResultItem> result = null;
            if (!_histogramItems.TryGetValue(timeslot, out result))
            {
                result = new Dictionary<string, ResultItem>();
                _histogramItems.Add(timeslot, result);
            }

            return result;
        }

        private int GetHistogramRowTimeSlot(DateTime requestTime)
        {
            double unixTime = requestTime.ToUnixTimeMs() / 1000.0;

            var resultTimeSlot = ((int) Math.Floor(unixTime / _aggregationStepSeconds)) * _aggregationStepSeconds;

            return resultTimeSlot;
        }

        public IEnumerable<HistogramResultRow> GetResults()
        {
            foreach (KeyValuePair<int, Dictionary<string, ResultItem>> histogramRow in _histogramItems.OrderBy(kv => kv.Key))
            {
                DateTime rowTime = DateTimeExtensions.UnixTimeToDateTime(histogramRow.Key);
                List<ResultItem> rowResultItems = histogramRow.Value.Select(kv => kv.Value).ToList();

                yield return new HistogramResultRow(rowTime, rowResultItems);
            }
        }

        public void Reset()
        {
            _histogramItems.Clear();
        }
    }
}