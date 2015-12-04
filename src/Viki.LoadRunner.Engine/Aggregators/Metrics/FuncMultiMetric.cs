using System;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class FuncMultiMetric<TValue> : MultiMetricBase<TValue>
    {
        private readonly Action<FlexiGrid<string, TValue>, TestContextResult> _metricProcedure;
        private readonly Func<TValue> _cellBuilderFunc;

        public FuncMultiMetric(Action<FlexiGrid<string,TValue>,TestContextResult> metricProcedure, Func<TValue> cellBuilderFunc)
            : base(cellBuilderFunc)
        {
            if (metricProcedure == null) throw new ArgumentNullException(nameof(metricProcedure));
            if (cellBuilderFunc == null) throw new ArgumentNullException(nameof(cellBuilderFunc));

            _metricProcedure = metricProcedure;
            _cellBuilderFunc = cellBuilderFunc;
        }

        protected override IMetric CreateNewMetric()
        {
            return new FuncMultiMetric<TValue>(_metricProcedure, _cellBuilderFunc);
        }

        protected override void AddResult(TestContextResult result)
        {
            _metricProcedure(_grid, result);
        }
    }
}