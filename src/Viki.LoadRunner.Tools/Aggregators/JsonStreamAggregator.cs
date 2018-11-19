﻿using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Tools.Extensions;

namespace Viki.LoadRunner.Tools.Aggregators
{
    public class JsonStreamAggregator : StreamAggregator
    {
        #region Fields

        private readonly Func<string> _outFileNameFunc;

        #endregion

        #region Constructors

        public JsonStreamAggregator(string jsonOutputfile) : this(() => jsonOutputfile)
        {
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

            results.SerializeToJson(fileName);
        }

        #endregion

        #region Replay functions

        public static void Replay(string jsonResultsFile, params IAggregator[] targetAggregators)
        {
            Replay<object>(jsonResultsFile, targetAggregators);
        }

        public static void Replay<TUserData>(string jsonResultsFile, params IAggregator[] targetAggregators)
        {
            IEnumerable<IResult> resultsStream = Load<TUserData>(jsonResultsFile);

            Replay(resultsStream, targetAggregators);
        }

        public static IEnumerable<ReplayResult<TUserData>> Load<TUserData>(string jsonResultsFile)
        {
            return JsonStream.DeserializeFromJson<ReplayResult<TUserData>>(jsonResultsFile);
        }

        #endregion
    }
}