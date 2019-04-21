using System;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline.Extensions;
using Viki.LoadRunner.Engine.Core.Counter.Interfaces;
using Viki.LoadRunner.Engine.Core.Factory;


// TODO: Performance can be increased here by creating seperate thread for each aggregator using EnumerableQueue
// and fill in each EnumerableQueue instance of each aggregator instead of waiting for each aggregator to process iteration on the same thread.
namespace Viki.LoadRunner.Engine.Core.Collector
{
    public class PipelineDataAggregator
    {
        private readonly IAggregator[] _aggregators;
        private readonly PipeMuxer<IResult> _muxer;
        public PipeDataCollectorFactory Factory { get; }

        private volatile bool _stopping = true;
        private Thread _processorThread;
        public Exception Error { get; private set; }

        public PipelineDataAggregator(IAggregator[] aggregators, IThreadPoolCounter counter)
        {
            _aggregators = aggregators ?? throw new ArgumentNullException(nameof(aggregators));

            _muxer = new PipeMuxer<IResult>();
            Factory = new PipeDataCollectorFactory(_muxer, counter);
            
        }

        public void Start()
        {
            if (_stopping == true)
            {
                _stopping = false;

                _muxer.Reset();

                Error = null;

                _processorThread = new Thread(ProcessorThreadFunction);
                _processorThread.Start();

                Array.ForEach(_aggregators, aggregator => aggregator.Begin());
            }
        }

        public void End()
        {
            if (_stopping == false)
            {
                _muxer.Complete();

                _stopping = true;
                _processorThread.Join();
                _processorThread = null;

                Array.ForEach(_aggregators, aggregator => aggregator.End());
            }
        }


        #region ProcessorThreadFunction()

        private void ProcessorThreadFunction()
        {
            int index = 0;
            try
            {
                foreach (IResult result in _muxer.ToEnumerable())
                {
                    for (index = 0; index < _aggregators.Length; index++)
                    {
                        _aggregators[index].Aggregate(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Error = new AggregatorException("One of registered aggregators has failed", _aggregators[index], ex);
                // Error gets triggered through heartbeat which is much better compared to AsyncAggregator
            }
        }

        #endregion

        #region IDisposable

        ~PipelineDataAggregator()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_processorThread?.IsAlive == true)
                _processorThread.Abort();
        }

        #endregion
    }
}