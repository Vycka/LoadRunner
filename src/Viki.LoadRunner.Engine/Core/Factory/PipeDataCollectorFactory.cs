using System;
using Viki.LoadRunner.Engine.Core.Collector;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline.Interfaces;
using Viki.LoadRunner.Engine.Core.Counter.Interfaces;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Factory
{
    public class PipeDataCollectorFactory : IDataCollectorFactory
    {
        private readonly IPipeFactory<IResult> _pipeFactory;
        private readonly IThreadPoolCounter _counter;

        public PipeDataCollectorFactory(IPipeFactory<IResult> pipeFactory, IThreadPoolCounter counter)
        {
            _pipeFactory = pipeFactory ?? throw new ArgumentNullException(nameof(pipeFactory));
            _counter = counter ?? throw new ArgumentNullException(nameof(counter));
        }

        public IDataCollector Create(IIterationResult iterationContext)
        {
            IProducer<IResult> pipe = _pipeFactory.Create();

            IDataCollector collector = new PipeDataCollector(pipe, iterationContext, _counter);

            return collector;
        }
    }
}