using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Engine.Aggregators
{
    /// <summary>
    /// Since TestContextResultReceived calls are synchronous from benchmarking threads, this class unloads processing to its own seperate thread
    /// It's already used in LoadRunnerEngine, so no need to reuse it again.
    /// </summary>
    internal class AsyncResultsAggregator : IResultsAggregator, IDisposable
    {
        #region Fields

        private readonly IResultsAggregator[] _resultsAggregators;
        private readonly ConcurrentQueue<IResult> _processingQueue = new ConcurrentQueue<IResult>();

        private volatile bool _stopping = true;
        private Thread _processorThread;
        private Exception _thrownException;

        #endregion

        #region Constructor

        public AsyncResultsAggregator(params IResultsAggregator[] resultsAggregators)
        {
            _resultsAggregators = resultsAggregators;
        }

        #endregion

        #region IResultsAggregator

        void IResultsAggregator.Begin()
        {
            if (_stopping == true)
            {
                _stopping = false;

                _processorThread = new Thread(ProcessorThreadFunction);
                _processorThread.Start();

                Parallel.ForEach(_resultsAggregators, aggregator => aggregator.Begin());
            }
        }

        void IResultsAggregator.End()
        {
            if (_stopping == false)
            {
                _stopping = true;
                _processorThread?.Join();

                Parallel.ForEach(_resultsAggregators, aggregator => aggregator.End());

                _processorThread = null;
            }
        }

        void IResultsAggregator.TestContextResultReceived(IResult result)
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
            Action<IResult> singleAggregator = r => _resultsAggregators[0].TestContextResultReceived(r);
            Action<IResult> multiAggregator = r => Parallel.ForEach(_resultsAggregators, a => a.TestContextResultReceived(r));

            Action<IResult> aggregator = _resultsAggregators.Length == 1 ? singleAggregator : multiAggregator;

            try
            {
                while (!_stopping || _processingQueue.IsEmpty == false)
                {
                    IResult resultObject;
                    while (_processingQueue.TryDequeue(out resultObject))
                    {
                        IResult localResultObject = resultObject;

                        aggregator(localResultObject);
                    }

                    Thread.Sleep(50);
                }
            }
            catch (Exception ex)
            {
                _thrownException = ex;
                throw;
            }
        }

        #endregion

        #region IDisposable

        ~AsyncResultsAggregator()
        {
            Dispose();
        }

        public void Dispose()
        {
            ((IResultsAggregator)this).End();
        }

        #endregion
    }
}
