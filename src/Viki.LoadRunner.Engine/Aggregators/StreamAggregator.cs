using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Result;
using Viki.LoadRunner.Engine.Utils;

namespace Viki.LoadRunner.Engine.Aggregators
{
    public class StreamAggregator : IResultsAggregator
    {
        #region Fields

        private readonly Action<IEnumerable<IResult>> _streamWriterAction;

        private readonly ConcurrentQueue<IResult> _queue = new ConcurrentQueue<IResult>();

        private Task _writerTask;
        private bool _stopTask;

        #endregion

        #region Constructor

        public StreamAggregator(Action<IEnumerable<IResult>> streamWriterAction)
        {
            if (streamWriterAction == null)
                throw new ArgumentNullException(nameof(streamWriterAction));

            _streamWriterAction = streamWriterAction;
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

        public static void Replay<TUserData>(IEnumerable<IResult> results, params IResultsAggregator[] targetAggregators)
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