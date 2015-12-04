using System;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    abstract public class MetricBase<TValue> : IMetric
    {
        protected readonly string _columnName;
        protected readonly TValue _initialvalue;
        protected TValue _value { get; }

        protected MetricBase(string keyName, TValue initialValue)
        {
            if (keyName == null)
                throw new ArgumentNullException(nameof(keyName));

            _columnName = keyName;
            _value = initialValue;
            _initialvalue = initialValue;
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

        string[] IMetric.ColumnNames => new[] { _columnName };
        object[] IMetric.Values => new[] { (object)_value };
    }
}