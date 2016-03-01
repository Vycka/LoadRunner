using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Viki.LoadRunner.Engine.Executor.Result;
using Viki.LoadRunner.Engine.Utils;

namespace Viki.LoadRunner.Engine.Aggregators
{
    /// <summary>
    /// StreamAggregator provides loadtest raw/masterdata (IResult) IEnumerable stream 
    /// </summary>
    public class StreamAggregator : IResultsAggregator
    {
        #region Fields

        protected Action<IEnumerable<IResult>> _streamWriterAction;

        private readonly ConcurrentQueue<IResult> _queue = new ConcurrentQueue<IResult>();

        private Task _writerTask;
        private bool _stopTask;

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

        protected StreamAggregator()
        {
        }

        #endregion

        #region EnumerableQueue

        private IEnumerable<IResult> EnumerableQueue
        {
            get
            {
                while (_stopTask == false || _queue.IsEmpty == false)
                {
                    IResult result;

                    while (_queue.TryDequeue(out result))
                        yield return result;

                    Thread.Sleep(100);
                }
            }
        }

        #endregion

        #region IResultsAggregator

        void IResultsAggregator.Begin()
        {
            _stopTask = false;

            _writerTask = new Task(() => _streamWriterAction(EnumerableQueue), TaskCreationOptions.LongRunning);
            _writerTask.Start();
        }

        void IResultsAggregator.TestContextResultReceived(IResult result)
        {
            if (_writerTask?.Exception != null)
                throw _writerTask.Exception;

            _queue.Enqueue(result);
        }

        void IResultsAggregator.End()
        {
            _stopTask = true;

            _writerTask?.Wait();
        }

        #endregion

        #region Replayer

        /// <summary>
        /// Replays raw result stream to provided aggregators.
        /// You can use this to replay saved masterdata of previously executed loadtest to differently configured aggregators - allowing to see the results from different angles.
        /// 
        /// Contact me in github if interested.
        /// </summary>
        /// <param name="results">LoadTest masterdata result stream</param>
        /// <param name="targetAggregators">Result aggregators</param>
        public static void Replay(IEnumerable<IResult> results, params IResultsAggregator[] targetAggregators)
        {
            targetAggregators.ForEach(aggregator => aggregator.Begin());

            foreach (IResult result in results)
            {
                targetAggregators.ForEach(aggregator => aggregator.TestContextResultReceived(result));
            }

            targetAggregators.ForEach(aggregator => aggregator.End());
        }

        #endregion
    }
}