using System;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Executor.Result;

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

        protected abstract IMetric CreateNewMetric();

        IMetric IMetric.CreateNew()
        {
            return CreateNewMetric();
        }

        protected abstract void AddResult(IResult result);

        void IMetric.Add(IResult result)
        {
            AddResult(result);
        }

        string[] IMetric.ColumnNames => new[] { _keyName };
        object[] IMetric.Values => new[] { (object)_value };
    }
}