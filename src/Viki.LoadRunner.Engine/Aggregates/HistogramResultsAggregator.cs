using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregates.Results;
using Viki.LoadRunner.Engine.Aggregates.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregates
{
    public class HistogramResultsAggregator : IResultsAggregator
    {
        private readonly int _aggregationStepSeconds;
        private readonly Dictionary<int, Dictionary<string, ResultItemRow>> _histogramItems = new Dictionary<int, Dictionary<string, ResultItemRow>>();

        private readonly static Checkpoint PreviousCheckpointBase = new Checkpoint("", TimeSpan.Zero);

        public HistogramResultsAggregator(int aggregationStepSeconds = 1)
        {
            _aggregationStepSeconds = aggregationStepSeconds;
        }

        public void TestContextResultReceived(TestContextResult result)
        {
            int histogramRowTimeSlot = GetHistogramRowTimeSlot(result.IterationFinished);
            Dictionary<string, ResultItemRow> histogramRow = GetHistogramRow(histogramRowTimeSlot);

            Checkpoint previousCheckpoint = PreviousCheckpointBase;
            foreach (Checkpoint currentCheckpoint in result.Checkpoints)
            {
                TimeSpan momentCheckpointTimeSpan = currentCheckpoint.TimePoint - previousCheckpoint.TimePoint;
                ResultItemRow checkpointResultObject = GetHistogramTargetResultObject(histogramRow, currentCheckpoint.CheckpointName);

                checkpointResultObject.AggregateResult(momentCheckpointTimeSpan, currentCheckpoint, result);
                previousCheckpoint = currentCheckpoint;
            }
        }

        private ResultItemRow GetHistogramTargetResultObject(Dictionary<string, ResultItemRow> histogramRow, string checkpointName)
        {
            ResultItemRow result = null;
            if (!histogramRow.TryGetValue(checkpointName, out result))
            {
                result = new ResultItemRow(checkpointName);
                histogramRow.Add(checkpointName, result);
            }

            return result;
        }

        private Dictionary<string, ResultItemRow> GetHistogramRow(int timeslot)
        {
            Dictionary<string, ResultItemRow> result = null;
            if (!_histogramItems.TryGetValue(timeslot, out result))
            {
                result = new Dictionary<string, ResultItemRow>();
                _histogramItems.Add(timeslot, result);
            }

            return result;
        }

        private int GetHistogramRowTimeSlot(DateTime requestTime)
        {
            double unixTime = requestTime.ToUnixTimeMs() / 1000.0;

            var resultTimeSlot = ((int)(unixTime / _aggregationStepSeconds)) * _aggregationStepSeconds;

            return resultTimeSlot;
        }

        public IEnumerable<HistogramResultRow> GetResults()
        {
            foreach (KeyValuePair<int, Dictionary<string, ResultItemRow>> histogramRow in _histogramItems.OrderBy(kv => kv.Key))
            {
                DateTime rowTime = DateTimeExtensions.UnixTimeToDateTime(histogramRow.Key);
                List<ResultItemRow> rowResultItems = histogramRow.Value.Select(kv => kv.Value).ToList();

                yield return new HistogramResultRow(rowTime, rowResultItems);
            }
        }

        public void Reset()
        {
            _histogramItems.Clear();
        }
    }
}