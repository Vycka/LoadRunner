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
        private readonly Dictionary<int, DefaultTestContextResultAggregate> _histogramItems = new Dictionary<int, DefaultTestContextResultAggregate>();

        public HistogramResultsAggregator(int aggregationStepSeconds = 1)
        {
            _aggregationStepSeconds = aggregationStepSeconds;
        }

        public void TestContextResultReceived(TestContextResult result)
        {
            _orderLearner.Learn(result);

            int histogramRowTimeSlot = GetHistogramRowTimeSlot(result.IterationFinished);
            DefaultTestContextResultAggregate histogramRowAggregate = GetHistogramRow(histogramRowTimeSlot);
            histogramRowAggregate.AggregateResult(result);
        }


        private DefaultTestContextResultAggregate GetHistogramRow(int timeslot)
        {
            DefaultTestContextResultAggregate result = null;
            if (!_histogramItems.TryGetValue(timeslot, out result))
            {
                result = new DefaultTestContextResultAggregate();
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
            foreach (KeyValuePair<int, DefaultTestContextResultAggregate> histogramItem in _histogramItems)
            {
                HistogramResultRow result = new HistogramResultRow(
                    UnixDateTimeExtensions.UnixTimeToDateTime(histogramItem.Key),
                    mapper.Map(histogramItem.Value, true).ToList()
                );

                yield return result;
            }
        }


        public void Begin()
        {
            _histogramItems.Clear();
        }

        public void End()
        {

        }
    }
}