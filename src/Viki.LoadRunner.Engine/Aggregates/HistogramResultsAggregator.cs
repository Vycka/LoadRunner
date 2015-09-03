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

        private readonly CheckpointOrderLearner _orderLearner = new CheckpointOrderLearner();
        private readonly Dictionary<int, Dictionary<string, AggregatedCheckpoint>> _histogramItems = new Dictionary<int, Dictionary<string, AggregatedCheckpoint>>();

        private readonly static Checkpoint PreviousCheckpointBase = new Checkpoint("", TimeSpan.Zero);

        public HistogramResultsAggregator(int aggregationStepSeconds = 1)
        {
            _aggregationStepSeconds = aggregationStepSeconds;
        }

        public void TestContextResultReceived(TestContextResult result)
        {
            _orderLearner.Learn(result);

            int histogramRowTimeSlot = GetHistogramRowTimeSlot(result.IterationFinished);
            Dictionary<string, AggregatedCheckpoint> histogramRow = GetHistogramRow(histogramRowTimeSlot);

            Checkpoint previousCheckpoint = PreviousCheckpointBase;
            foreach (Checkpoint currentCheckpoint in result.Checkpoints)
            {
                TimeSpan momentCheckpointTimeSpan = currentCheckpoint.TimePoint - previousCheckpoint.TimePoint;
                AggregatedCheckpoint checkpointResultObject = GetHistogramTargetResultObject(histogramRow, currentCheckpoint.CheckpointName);

                checkpointResultObject.AggregateCheckpoint(momentCheckpointTimeSpan, currentCheckpoint, result);
                previousCheckpoint = currentCheckpoint;
            }
        }

        private AggregatedCheckpoint GetHistogramTargetResultObject(Dictionary<string, AggregatedCheckpoint> histogramRow, string checkpointName)
        {
            AggregatedCheckpoint result = null;
            if (!histogramRow.TryGetValue(checkpointName, out result))
            {
                result = new AggregatedCheckpoint(checkpointName);
                histogramRow.Add(checkpointName, result);
            }

            return result;
        }

        private Dictionary<string, AggregatedCheckpoint> GetHistogramRow(int timeslot)
        {
            Dictionary<string, AggregatedCheckpoint> result = null;
            if (!_histogramItems.TryGetValue(timeslot, out result))
            {
                result = new Dictionary<string, AggregatedCheckpoint>();
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
            foreach (KeyValuePair<int, Dictionary<string, AggregatedCheckpoint>> histogramItem in _histogramItems)
            {
                HistogramResultRow result = new HistogramResultRow(
                    DateTimeExtensions.UnixTimeToDateTime(histogramItem.Key),
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