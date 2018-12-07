using System;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline.Interfaces;
using Viki.LoadRunner.Engine.Core.Counter.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Collector
{
    public class PipeDataCollector : IDataCollector
    {
        private readonly IProducer<IResult> _producer;

        private readonly IIterationResult _context;
        private readonly IThreadPoolCounter _poolStats;

        public PipeDataCollector(IProducer<IResult> producer, IIterationResult context, IThreadPoolCounter poolStats)
        {
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _poolStats = poolStats ?? throw new ArgumentNullException(nameof(poolStats));
        }

        public void Collect()
        {
            _producer.Produce(new IterationResult(_context, _poolStats));
        }

        public void Complete()
        {
            _producer.ProducingCompleted();
        }
    }
}