using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Result;
using Viki.LoadRunner.Tools.Utils;

namespace Viki.LoadRunner.Tools.Aggregators
{
    public class JsonStreamAggregator : StreamAggregator
    {
        #region Fields

        private readonly Func<string> _outFileNameFunc;

        #endregion

        #region Constructors

        public JsonStreamAggregator(string jsonOutputfile)
        {
            _outFileNameFunc = () => jsonOutputfile;

            _streamWriterAction = StreamWriterFunction;
        }

        public JsonStreamAggregator(Func<string> dynamicJsonOutputFile)
        {
            _outFileNameFunc = dynamicJsonOutputFile;

            _streamWriterAction = StreamWriterFunction;
        }

        #endregion

        #region Write function

        private void StreamWriterFunction(IEnumerable<IResult> results)
        {
            string fileName = _outFileNameFunc();

            results.SerializeSequenceToJson(fileName);
        }

        #endregion

        #region Replay functions

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

        #endregion
    }
}