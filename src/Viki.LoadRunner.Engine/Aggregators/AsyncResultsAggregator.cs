using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators
{
    /// <summary>
    /// Since TestContextResultReceived call are synchronous from benchmarking threads, this class unloads processing to its own seperate thread
    /// It's already used in LoadRunnerEngine, so no need to reuse it again.
    /// </summary>
    internal class AsyncResultsAggregator : IResultsAggregator, IDisposable
    {
        private readonly IResultsAggregator[] _resultsAggregators;
        private readonly ConcurrentQueue<TestContextResult> _processingQueue = new ConcurrentQueue<TestContextResult>();
        
        private volatile bool _stopping;
        private Thread _processorThread;
        private Exception _thrownException = null;

        public AsyncResultsAggregator(params IResultsAggregator[] resultsAggregators)
        {
            _resultsAggregators = resultsAggregators;
        }

        public void Begin(DateTime testBeginTime)
        {
            _stopping = false;

            _processorThread = new Thread(ProcessorThreadFunction);
            _processorThread.Start();

            Parallel.ForEach(_resultsAggregators, aggregator => aggregator.Begin(testBeginTime));
        }

        public void End()
        {
            _stopping = true;
            _processorThread.Join();

            Parallel.ForEach(_resultsAggregators, aggregator => aggregator.End());
        }


        public void Dispose()
        {
            End();
        }

        public void TestContextResultReceived(TestContextResult result)
        {
            _processingQueue.Enqueue(result);

            // ATM Exception will be cought in worker thread and bubbles up to the ThreadCoordinator, where it gets rethrown to the Main thread
            // TODO: think of something better.
            if (_thrownException != null)
                throw _thrownException;
        }

        // TODO: Think of better way to catch error
        private void ProcessorThreadFunction()
        {
            try
            {
                bool onlyOneAggregator = _resultsAggregators.Length == 1;

                while (!_stopping || _processingQueue.IsEmpty == false)
                {
                    TestContextResult resultObject;
                    while (_processingQueue.TryDequeue(out resultObject))
                    {
                        TestContextResult localResultObject = resultObject;

                        if (onlyOneAggregator)
                            _resultsAggregators[0].TestContextResultReceived(localResultObject);
                        else
                            Parallel.ForEach(_resultsAggregators, aggregator => aggregator.TestContextResultReceived(localResultObject));
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
    }
}
