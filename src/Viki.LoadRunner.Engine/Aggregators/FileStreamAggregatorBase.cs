using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators
{
    public abstract class FileStreamAggregatorBase : StreamAggregatorBase
    {
        #region Fields

        private readonly Func<string> _outFileNameFunc;

        #endregion

        #region Constructors

        public FileStreamAggregatorBase(string jsonOutputfile) : this(() => jsonOutputfile)
        {
        }

        public FileStreamAggregatorBase(Func<string> dynamicJsonOutputFile)
        {
            _outFileNameFunc = dynamicJsonOutputFile;

        }

        #endregion

        #region FileStreamAggregatorBase

        protected override void Process(IEnumerable<IResult> stream)
        {
            Write(_outFileNameFunc(), stream);
        }

        #endregion

        #region FileStreamAggregatorBase abstracts

        public abstract void Write(string filePath, IEnumerable<IResult> stream);

        #endregion
    }
}