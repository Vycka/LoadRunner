using System;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Executor.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class FuncMetric<TValue> : MetricBase<TValue>
    {
        private readonly Func<TValue, IResult, TValue> _metricFunc;

        public FuncMetric(string keyName, TValue initialValue, Func<TValue, IResult, TValue> metricFunc) 
            : base(keyName, initialValue)
        {
            _metricFunc = metricFunc;
        }

        protected override IMetric CreateNewMetric()
        {
            return new FuncMetric<TValue>(_keyName, _initialValue, _metricFunc);
        }

        protected override void AddResult(IResult result)
        {
            _value = _metricFunc(_value, result);
        }
    }
}