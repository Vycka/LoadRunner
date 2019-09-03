using System.Collections.Generic;

namespace Viki.LoadRunner.Engine.Analytics.Interfaces
{
    public interface IHistogramBuilder<TData, THistogram>
    {
        List<IDimension<TData>> Dimensions { get; }
        List<IMetric<TData>> Metrics { get; }

        Dictionary<string, string> ColumnAliases { get; }
        List<string> ColumnIgnoreNames { get; }

        Dictionary<int, PostProcessDelegate> MetricsPostProcess { get; }
    }

}