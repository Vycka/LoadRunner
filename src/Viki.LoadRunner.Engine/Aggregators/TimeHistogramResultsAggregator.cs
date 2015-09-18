using System;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators
{
    public class TimeHistogramResultsAggregator : HistogramResultsAggregator
    {
        private readonly int _aggregationStepSeconds;
        private long _testBeginTimeMs;

        public TimeHistogramResultsAggregator(int aggregationStepSeconds = 1)
        {
            _aggregationStepSeconds = aggregationStepSeconds;
            _groupByKeyCalculatorFunction = GroupByCalculatorFunction;
        }

        public override void Begin(DateTime testBeginTime)
        {
            _testBeginTimeMs = testBeginTime.ToUnixTimeMs();

            base.Begin(testBeginTime);
        }

        private object GroupByCalculatorFunction(TestContextResult result)
        {
            double iterationEndTime = (result.IterationFinished.ToUnixTimeMs() - _testBeginTimeMs) / 1000.0;

            var resultTimeSlot = ((int)(iterationEndTime / _aggregationStepSeconds)) * _aggregationStepSeconds;

            return resultTimeSlot;
        }
    }
}