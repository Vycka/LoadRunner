using System;
using System.Collections.Generic;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline;
using Viki.LoadRunner.Engine.Core.Counter.Interfaces;
using Viki.LoadRunner.Engine.Core.Factory;

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
                _stopping = true;
                _processorThread?.Join();

                Array.ForEach(_aggregators, aggregator => aggregator.End());

                _processorThread = null;
            }
        }


        #region ProcessorThreadFunction()

        // TODO: Think of better way to catch error (not using _thrownException)
        private void ProcessorThreadFunction()
        {
            int index = 0;
            try
            {
                while (!_stopping || _muxer.Available)
                {
                    IEnumerator<IReadOnlyList<IResult>> enumerator = _muxer.LockBatches().GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        do
                        {
                            IReadOnlyList<IResult> batch = enumerator.Current;
                            for (int i = 0; i < batch.Count; i++)
                            {
                                for (index = 0; index < _aggregators.Length; index++)
                                {
                                    _aggregators[index].Aggregate(batch[i]);
                                }
                            }

                        } while (enumerator.MoveNext());
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                    enumerator.Dispose();
                    _muxer.ReleaseBatches();
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