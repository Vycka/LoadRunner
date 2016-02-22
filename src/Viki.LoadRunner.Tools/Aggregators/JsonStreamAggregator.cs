using System.Collections.Generic;
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
            IEnumerable<IResult> resultsStream = Load<TUserData>(jsonResultsFile);

            Replay(resultsStream, targetAggregators);
        }

        public static IEnumerable<ReplayResult<TUserData>> Load<TUserData>(string jsonResultsFile)
        {
            return JsonStream.DeserializeSequenceFromJson<ReplayResult<TUserData>>(jsonResultsFile);
        }
    }
}