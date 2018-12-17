using System.Collections.Generic;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Utils
{
    public static class ResultExtensions
    {
        public static void Replay(this IEnumerable<IResult> results, params IAggregator[] agregators)
        {
            StreamAggregator.Replay(results, agregators);
        }
    }
}