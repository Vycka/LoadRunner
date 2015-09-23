using System.Collections.Generic;
using Viki.LoadRunner.Engine.Aggregators.Aggregates;

namespace Viki.LoadRunner.Engine.Aggregators.Results
{
    public class HistogramResultRow
    {
        private readonly List<ResultItemRow> _resultItems;

        public readonly object GroupByKey;
        public readonly double CreatedThreads;
        public readonly double WorkingThreads;

        public IReadOnlyList<ResultItemRow> ResultItems => _resultItems;

        public HistogramResultRow(object groupByKey, TestContextResultAggregate resultsAggregate, List<ResultItemRow> resultItems)
        {
            CreatedThreads = resultsAggregate.CreatedThreadsAvg;
            WorkingThreads = resultsAggregate.WorkingThreadsAvg;

            GroupByKey = groupByKey;
            _resultItems = resultItems;
        }
    }
}