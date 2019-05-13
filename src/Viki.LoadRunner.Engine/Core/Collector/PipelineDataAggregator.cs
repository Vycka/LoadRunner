using System;
using System.Collections.Generic;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline.Extensions;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline.Interfaces;


namespace Viki.LoadRunner.Engine.Core.Collector
{
    public class PipelineDataAggregator : IDisposable
    {
        private readonly IAggregator[] _aggregators;
        private readonly IPipeProvider<IResult> _pipeProvider;
        private readonly PipeMuxer<IResult> _muxer;

        private volatile bool _stopping = true;
        private Thread _processorThread;
        public Exception Error { get; private set; }

        public PipelineDataAggregator(IAggregator[] aggregators, IPipeProvider<IResult> pipeProvider)
        {
            _aggregators = aggregators ?? throw new ArgumentNullException(nameof(aggregators));
            _pipeProvider = pipeProvider ?? throw new ArgumentNullException(nameof(pipeProvider));

            _muxer = new PipeMuxer<IResult>();
            pipeProvider.PipeCreatedEvent += OnPipeCreatedEvent; 
            
        }

        private void OnPipeCreatedEvent(IPipeFactory<IResult> sender, BatchingPipe<IResult> pipe)
        {
            _muxer.Add(pipe);
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
            IResult row = null;
            try
            {
                using (IEnumerator<IResult> enumerator = _muxer.ToEnumerable().GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        row = enumerator.Current;
                        for (index = 0; index < _aggregators.Length; index++)
                        {
                            _aggregators[index].Aggregate(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error = new AggregatorException("One of registered aggregators has failed", _aggregators[index], row, ex);
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

            _pipeProvider.PipeCreatedEvent -= OnPipeCreatedEvent;
        }

        #endregion
    }
}