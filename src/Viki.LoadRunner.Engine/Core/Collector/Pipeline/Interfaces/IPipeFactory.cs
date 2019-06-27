namespace Viki.LoadRunner.Engine.Core.Collector.Pipeline.Interfaces
{
    public interface IPipeFactory<T> : IPipeProvider<T>
    {
        BatchingPipe<T> Create(); 
    }

    public interface IPipeProvider<T>
    {
        event PipeCreatedEventDelegate<T> PipeCreatedEvent;
    }

    public delegate void PipeCreatedEventDelegate<T>(IPipeFactory<T> sender, BatchingPipe<T> pipe);

}