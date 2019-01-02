using System;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class FuncMultiMetric<TValue> : MultiMetricBase<TValue>
    {
        private readonly Action<FlexiRow<string, TValue>, IResult> _metricProcedure;
        private readonly Func<TValue> _cellBuilderFunc;

        public FuncMultiMetric(Action<FlexiRow<string,TValue>, IResult> metricProcedure, Func<TValue> cellBuilderFunc)
            : base(cellBuilderFunc)
        {
            if (metricProcedure == null) throw new ArgumentNullException(nameof(metricProcedure));
            if (cellBuilderFunc == null) throw new ArgumentNullException(nameof(cellBuilderFunc));

            _metricProcedure = metricProcedure;
            _cellBuilderFunc = cellBuilderFunc;
        }

        protected override IMetric<IResult> CreateNewMetric()
        {
            return new FuncMultiMetric<TValue>(_metricProcedure, _cellBuilderFunc);
        }

        protected override void AddResult(IResult result)
        {
            _metricProcedure(_row, result);
        }
    }
}