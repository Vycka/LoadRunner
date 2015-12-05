using System;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class FuncMetric<TValue> : MetricBase<TValue>
    {
        private readonly Func<TValue, TestContextResult, TValue> _metricFunc;

        public FuncMetric(string keyName, TValue initialValue, Func<TValue, TestContextResult, TValue> metricFunc) 
            : base(keyName, initialValue)
        {
            _metricFunc = metricFunc;
        }

        protected override IMetric CreateNewMetric()
        {
            return new FuncMetric<TValue>(_keyName, _initialValue, _metricFunc);
        }

        protected override void AddResult(TestContextResult result)
        {
            _value = _metricFunc(_value, result);
        }
    }
}