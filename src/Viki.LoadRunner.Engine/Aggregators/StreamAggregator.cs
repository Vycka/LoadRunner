using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Engine.Aggregators
{
    public class StreamAggregator : IResultsAggregator
    {
        private readonly Action<IEnumerable<IResult>> _streamWriterAction;

        private readonly ConcurrentQueue<IResult> _queue = new ConcurrentQueue<IResult>();

        private Task _writerTask;
        private bool _stopTask;

        public StreamAggregator(Action<IEnumerable<IResult>> streamWriterAction)
        {
            if (streamWriterAction == null)
                throw new ArgumentNullException(nameof(streamWriterAction));

            _streamWriterAction = streamWriterAction;
        }

        private IEnumerable<IResult> _queueStream
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

        void IResultsAggregator.Begin()
        {
            _stopTask = false;

            _writerTask = new Task(() => _streamWriterAction(_queueStream), TaskCreationOptions.LongRunning);
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
    }
}