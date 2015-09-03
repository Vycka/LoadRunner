using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Viki.LoadRunner.Engine.Aggregates.Results;

namespace Viki.LoadRunner.Engine.Aggregates.Utils
{
    [Serializable]
    public class HistogramResultRow
    {
        private readonly List<ResultItemRow> _resultItems;
            
        [DataMember]
        public readonly DateTime TimePoint;
        [DataMember]
        public IReadOnlyList<ResultItemRow> ResultItems => _resultItems;


        public HistogramResultRow(DateTime timePoint, List<ResultItemRow> resultItems)
        {
            TimePoint = timePoint;
            _resultItems = resultItems;
        }
    }
}