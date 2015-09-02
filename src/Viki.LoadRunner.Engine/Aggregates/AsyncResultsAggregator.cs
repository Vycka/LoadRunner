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

        public void Start()
        {
            _stopping = false;

            _processorThread = new Thread(ProcessorThreadFunction);
            _processorThread.Start();
        }

        public void Stop()
        {
            _stopping = true;
            _processorThread.Join();
        }


        public void Dispose()
        {
            Stop();
        }

        public void TestContextResultReceived(TestContextResult result)
        {
            _processingQueue.Enqueue(result);
        }

        private void ProcessorThreadFunction()
        {
            while (!_stopping)
            {
                TestContextResult resultObject;
                while (_processingQueue.TryDequeue(out resultObject))
                {
                    TestContextResult localResultObject = resultObject;
                    Parallel.ForEach(_resultsAggregators, aggregator => aggregator.TestContextResultReceived(localResultObject));
                }

                Thread.Sleep(50);
            }
        }
    }
}