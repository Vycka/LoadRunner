using System;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class FuncMetric<TValue> : MetricBase<TValue>
    {
        private readonly Action<TValue, TestContextResult> _metricProcedure;

        public FuncMetric(string keyName, TValue initialValue, Action<TValue, TestContextResult> metricProcedure) 
            : base(keyName, initialValue)
        {
            _metricProcedure = metricProcedure;
        }

        protected override IMetric CreateNewMetric()
        {
            return new FuncMetric<TValue>(_columnName, _initialvalue, _metricProcedure);
        }

        protected override void AddResult(TestContextResult result)
        {
            _metricProcedure(_value, result);
        }
    }
}