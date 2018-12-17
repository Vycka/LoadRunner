using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Utils;

namespace Viki.LoadRunner.Engine.Aggregators
{
    /// <summary>
    /// StreamAggregator provides loadtest raw/masterdata (IResult) IEnumerable stream 
    /// </summary>
    public class StreamAggregator : StreamAggregatorBase
    {
        #region Fields

        protected Action<IEnumerable<IResult>> _streamWriterAction;

        #endregion

        #region Constructor

        /// <summary>
        /// StreamAggregator provides loadtest raw/masterdata (IResult) IEnumerable stream 
        /// </summary>
        /// <param name="streamWriterAction">Action, which will be called, when its required. that given IEnumerable&lt;IResult&gt; won't return, until loadtest is over.</param>
        public StreamAggregator(Action<IEnumerable<IResult>> streamWriterAction)
        {
            if (streamWriterAction == null)
                throw new ArgumentNullException(nameof(streamWriterAction));

            _streamWriterAction = streamWriterAction;
        }

        #endregion

        #region Override

        protected override void Process(IEnumerable<IResult> stream) => _streamWriterAction(stream);

        #endregion

        #region Replayer

        /// <summary>
        /// Replays raw result stream to provided aggregators.
        /// You can use this to replay saved masterdata of previously executed loadtest to differently configured aggregators - allowing to see the results from different angles.
        /// </summary>
        /// <param name="results">LoadTest masterdata result stream</param>
        /// <param name="aggregators">Result aggregators</param>
        public static void Replay(IEnumerable<IResult> results, params IAggregator[] aggregators)
        {
            aggregators.ForEach(aggregator => aggregator.Begin());

            foreach (IResult result in results)
            {
                aggregators.ForEach(aggregator => aggregator.Aggregate(result));
            }

            aggregators.ForEach(aggregator => aggregator.End());
        }

        #endregion
    }
}