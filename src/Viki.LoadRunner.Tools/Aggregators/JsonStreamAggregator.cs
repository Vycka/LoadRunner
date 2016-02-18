using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Result;
using Viki.LoadRunner.Tools.Utils;

namespace Viki.LoadRunner.Tools.Aggregators
{
    public class JsonStreamAggregator : StreamAggregator
    {
        public JsonStreamAggregator(string jsonOutputfile) : base(results => results.SerializeSequenceToJson(jsonOutputfile))
        {
        }

        public static void Replay(string jsonResultsFile, params IResultsAggregator[] targetAggregators)
        {
            Replay<object>(jsonResultsFile, targetAggregators);
        }

        public static void Replay<TUserData>(string jsonResultsFile, params IResultsAggregator[] targetAggregators)
        {
            IEnumerable<IResult> resultsStream = JsonStream
                .DeserializeSequenceFromJson<ReplayResult<TUserData>>(jsonResultsFile)
                .Select(r => (IResult) r);

            StreamAggregator.Replay(resultsStream, targetAggregators);
        }
    }
}