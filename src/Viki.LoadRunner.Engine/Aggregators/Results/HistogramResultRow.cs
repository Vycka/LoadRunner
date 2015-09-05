using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Aggregators.Aggregates;

namespace Viki.LoadRunner.Engine.Aggregators.Results
{
    public class HistogramResultRow
    {
        private readonly List<ResultItemRow> _resultItems;

        public readonly DateTime TimePoint;
        public readonly double CreatedThreads;
        public readonly double WorkingThreads;

        public IReadOnlyList<ResultItemRow> ResultItems => _resultItems;

        public HistogramResultRow(DefaultTestContextResultAggregate resultsAggregate, DateTime timePoint, List<ResultItemRow> resultItems)
        {
            CreatedThreads = resultsAggregate.CreatedThreadsAvg;
            WorkingThreads = resultsAggregate.WorkingThreadsAvg;

            TimePoint = timePoint;
            _resultItems = resultItems;
        }
    }
}