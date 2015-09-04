using System.Collections.Generic;

namespace Viki.LoadRunner.Engine.Aggregators.Results
{
    public class ResultsContainer
    {
        public readonly List<ResultItemRow> Results;
        public readonly ResultItemTotals Totals;


        public ResultsContainer(List<ResultItemRow> results, ResultItemTotals totals)
        {
            Results = results;
            Totals = totals;
        }
    }
}