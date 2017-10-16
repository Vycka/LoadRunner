using System;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public abstract class MultiMetricBase<TValue> : IMetric
    {
        protected readonly FlexiRow<string, TValue> _row;

        protected MultiMetricBase(Func<TValue> cellBuilderFunc)
        {
            _row = new FlexiRow<string, TValue>(cellBuilderFunc);
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

        string[] IMetric.ColumnNames => _row.Keys.ToArray();
        object[] IMetric.Values => _row.Values.Select(v => (object) v).ToArray();
    }
}