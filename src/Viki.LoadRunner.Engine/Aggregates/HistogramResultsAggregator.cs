using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregates.Aggregates;
using Viki.LoadRunner.Engine.Aggregates.Results;
using Viki.LoadRunner.Engine.Aggregates.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregates
{
    public class HistogramResultsAggregator : IResultsAggregator
    {
        private readonly int _aggregationStepSeconds;

        private readonly CheckpointOrderLearner _orderLearner = new CheckpointOrderLearner();
        private readonly Dictionary<int, Dictionary<string, CheckpointAggregate>> _histogramItems = new Dictionary<int, Dictionary<string, CheckpointAggregate>>();

        private readonly static Checkpoint PreviousCheckpointBase = new Checkpoint("", TimeSpan.Zero);

        public HistogramResultsAggregator(int aggregationStepSeconds = 1)
        {
            _aggregationStepSeconds = aggregationStepSeconds;
        }

        public void TestContextResultReceived(TestContextResult result)
        {
            _orderLearner.Learn(result);

            int histogramRowTimeSlot = GetHistogramRowTimeSlot(result.IterationFinished);
            Dictionary<string, CheckpointAggregate> histogramRow = GetHistogramRow(histogramRowTimeSlot);

            Checkpoint previousCheckpoint = PreviousCheckpointBase;
            foreach (Checkpoint currentCheckpoint in result.Checkpoints)
            {
                TimeSpan momentCheckpointTimeSpan = currentCheckpoint.TimePoint - previousCheckpoint.TimePoint;
                CheckpointAggregate checkpointAggregateResultObject = GetHistogramTargetResultObject(histogramRow, currentCheckpoint.CheckpointName);

                checkpointAggregateResultObject.AggregateCheckpoint(momentCheckpointTimeSpan, currentCheckpoint, result);
                previousCheckpoint = currentCheckpoint;
            }
        }

        private CheckpointAggregate GetHistogramTargetResultObject(Dictionary<string, CheckpointAggregate> histogramRow, string checkpointName)
        {
            CheckpointAggregate result = null;
            if (!histogramRow.TryGetValue(checkpointName, out result))
            {
                result = new CheckpointAggregate(checkpointName);
                histogramRow.Add(checkpointName, result);
            }

            return result;
        }

        private Dictionary<string, CheckpointAggregate> GetHistogramRow(int timeslot)
        {
            Dictionary<string, CheckpointAggregate> result = null;
            if (!_histogramItems.TryGetValue(timeslot, out result))
            {
                result = new Dictionary<string, CheckpointAggregate>();
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
            ResultsMapper mapper = new ResultsMapper(_orderLearner);
            foreach (KeyValuePair<int, Dictionary<string, CheckpointAggregate>> histogramItem in _histogramItems)
            {
                HistogramResultRow result = new HistogramResultRow(
                    UnixDateTimeExtensions.UnixTimeToDateTime(histogramItem.Key),
                    mapper.Map(histogramItem.Value).ToList()
                );

                yield return result;
            }
        }

        public void Reset()
        {
            _histogramItems.Clear();
        }
    }
}