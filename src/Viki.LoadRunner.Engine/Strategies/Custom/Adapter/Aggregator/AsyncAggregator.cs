using System;
using System.Collections.Concurrent;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Adapter.Aggregator
{
    /// <summary>
    /// Since TestContextResultReceived calls are synchronous from benchmarking threads, this class unloads processing to its own seperate thread
    /// It's already used in LoadRunnerEngine, so no need to reuse it again.
    /// </summary>
    internal class AsyncAggregator : IAggregator, IDisposable
    {
        #region Fields

        private readonly IAggregator[] _aggregators;
        private readonly ConcurrentQueue<IResult> _processingQueue = new ConcurrentQueue<IResult>();

        private volatile bool _stopping = true;
        private Thread _processorThread;
        private Exception _thrownException;

        #endregion

        #region Constructor

        public AsyncAggregator(params IAggregator[] aggregators)
        {
            _aggregators = aggregators;
        }

        #endregion

        #region IResultsAggregator

        void IAggregator.Begin()
        {
            if (_stopping == true)
            {
                _stopping = false;

                _processorThread = new Thread(ProcessorThreadFunction);
                _processorThread.Start();

                Array.ForEach(_aggregators, aggregator => aggregator.Begin());
            }
        }

        void IAggregator.End()
        {
            if (_stopping == false)
            {
                _stopping = true;
                _processorThread?.Join();

                Array.ForEach(_aggregators, aggregator => aggregator.End());

                _processorThread = null;
            }
        }

        void IAggregator.Aggregate(IResult result)
        {
            _processingQueue.Enqueue(result);

            // Exceptions thrown here goes to worker-thread who called this event
            // Worker-thread crashes because of that and then ThreadCoordinator detects this error on the main thread.
            // TODO: think of something better.
            if (_thrownException != null)
                throw _thrownException;
        }

        #endregion

        #region ProcessorThreadFunction()

        // TODO: Think of better way to catch error (not using _thrownException)
        private void ProcessorThreadFunction()
        {

            try
            {
                while (!_stopping || _processingQueue.IsEmpty == false)
                {
                    IResult resultObject;
                    while (_processingQueue.TryDequeue(out resultObject))
                    {
                        IResult localResultObject = resultObject;

                        Array.ForEach(_aggregators, a => a.Aggregate(localResultObject));
                    }

                    Thread.Sleep(100);
                }
            }
            catch (AggregateException ex)
            {
                _thrownException = ex;

                //throw ex.InnerException;

                // Rethrowing exception here does nothing, since this is executed in _processorThread
                // But it will bubble up to application-thread if LoadRunnerUi(IResultsAggregator) is used. (Because it invokes controls from this _processorThread, and this is the point where exceptions can reach the application-thread)
                // Not rethrowing here just makes LoadRunner work consistently.
            }
        }

        #endregion

        #region IDisposable

        ~AsyncAggregator()
        {
            Dispose();
        }

        public void Dispose()
        {
            ((IAggregator)this).End();
        }

        #endregion
    }
}
