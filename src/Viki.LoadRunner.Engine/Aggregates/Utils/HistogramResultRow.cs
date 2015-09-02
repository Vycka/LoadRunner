using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Viki.LoadRunner.Engine.Aggregates.Utils
{
    [Serializable]
    public class HistogramResultRow
    {
        private readonly List<ResultItem> _resultItems;
            
        [DataMember]
        public readonly DateTime TimePoint;
        [DataMember]
        public IReadOnlyList<ResultItem> ResultItems => _resultItems;


        public HistogramResultRow(DateTime timePoint, List<ResultItem> resultItems)
        {
            TimePoint = timePoint;
            _resultItems = resultItems;
        }
    }
}