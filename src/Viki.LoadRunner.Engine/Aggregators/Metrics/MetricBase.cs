using System;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public abstract class MetricBase<TValue> : IMetric
    {
        protected readonly string _keyName;
        protected readonly TValue _initialValue;
        protected TValue _value;

        protected MetricBase(string keyName, TValue initialValue)
        {
            if (keyName == null)
                throw new ArgumentNullException(nameof(keyName));

            _keyName = keyName;
            _value = initialValue;
            _initialValue = initialValue;
        }

        protected abstract IMetric<IResult> CreateNewMetric();

        IMetric<IResult> IMetric<IResult>.CreateNew()
        {
            return CreateNewMetric();
        }

        protected abstract void AddResult(IResult result);

        void IMetric<IResult>.Add(IResult result)
        {
            AddResult(result);
        }

        string[] IMetric<IResult>.ColumnNames => new[] { _keyName };
        object[] IMetric<IResult>.Values => new[] { (object)_value };
    }
}