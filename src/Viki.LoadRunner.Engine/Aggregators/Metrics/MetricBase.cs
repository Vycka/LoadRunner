using System;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    abstract public class MetricBase<TValue> : IMetric
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

        protected abstract void AddResult(TestContextResult result);

        void IMetric.Add(TestContextResult result)
        {
            AddResult(result);
        }

        string[] IMetric.ColumnNames => new[] { _keyName };
        object[] IMetric.Values => new[] { (object)_value };
    }
}