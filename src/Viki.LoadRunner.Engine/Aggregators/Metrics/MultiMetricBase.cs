using System;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public abstract class MultiMetricBase<TValue> : IMetric
    {
        protected readonly FlexiGrid<string, TValue> _grid;

        protected MultiMetricBase(Func<TValue> cellBuilderFunc)
        {
            _grid = new FlexiGrid<string, TValue>(cellBuilderFunc);
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

        string[] IMetric.ColumnNames => _grid.Keys.ToArray();
        object[] IMetric.Values => _grid.Values.Select(v => (object) v).ToArray();
    }
}