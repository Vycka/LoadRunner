using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregates
{
    /// <summary>
    /// Since TestContextResultReceived call are synchronous from benchmarking threads, this class unloads processing to its own seperate thread
    /// It's already used in LoadTestClient, so no need to reuse it again.
    /// </summary>
    internal class AsyncResultsAggregator : IResultsAggregator, IDisposable
    {
        private readonly IResultsAggregator[] _resultsAggregators;
        private readonly ConcurrentQueue<TestContextResult> _processingQueue = new ConcurrentQueue<TestContextResult>();
        
        private volatile bool _stopping;
        private Thread _processorThread;

        public AsyncResultsAggregator(params IResultsAggregator[] resultsAggregators)
        {
            _resultsAggregators = resultsAggregators;
        }

        public void Begin()
        {
            _stopping = false;

            _processorThread = new Thread(ProcessorThreadFunction);
            _processorThread.Start();
        }

        public void End()
        {
            _stopping = true;
            _processorThread.Join();
        }


        public void Dispose()
        {
            End();
        }

        public void TestContextResultReceived(TestContextResult result)
        {
            _processingQueue.Enqueue(result);
        }

        public void Reset()
        {
        }

        private void ProcessorThreadFunction()
        {
            bool onlyOneAggregator = _resultsAggregators.Length == 1;

            while (!_stopping)
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
    }
}