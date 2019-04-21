using Viki.LoadRunner.Engine.Core.Collector.Pipeline.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Collector.Pipeline
{
    public class PipeFactory<T> : IPipeFactory<T>
    {
        public BatchingPipe<T> Create()
        {
            BatchingPipe<T> pipe = new BatchingPipe<T>();

            PipeCreatedEvent?.Invoke(this, pipe);

            return pipe;
        }

        public event PipeFactoryDelegates.Created<T> PipeCreatedEvent;
    }
}