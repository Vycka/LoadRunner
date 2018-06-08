using System;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public abstract class MultiMetricBase<TValue> : IMetric
    {
        protected readonly FlexiRow<string, TValue> _row;

        protected MultiMetricBase(Func<TValue> cellBuilderFunc)
        {
            _row = new FlexiRow<string, TValue>(cellBuilderFunc);
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

        string[] IMetric<IResult>.ColumnNames => _row.Keys.ToArray();
        object[] IMetric<IResult>.Values => _row.Values.Select(v => (object) v).ToArray();
    }
}