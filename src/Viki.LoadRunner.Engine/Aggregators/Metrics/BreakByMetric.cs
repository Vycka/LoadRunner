using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Analytics.Metrics;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    /// <summary>
    /// BreakByMetric allows additional data slicing by provided sub-dimension.
    /// </summary>
    public class BreakByMetric :  SubDimension<IResult>
    {
        public BreakByMetric(IDimension<IResult> subDimension, params IMetric<IResult>[] actualMetrics) 
            : base(subDimension, actualMetrics)
        {
        }

        public BreakByMetric(IDimension<IResult> subDimension, ColumnNameDelegate columnNameSelector, params IMetric<IResult>[] actualMetrics)
            : base(subDimension, columnNameSelector, actualMetrics)
        {
        }
    }
}