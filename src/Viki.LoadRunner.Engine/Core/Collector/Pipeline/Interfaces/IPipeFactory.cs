namespace Viki.LoadRunner.Engine.Core.Collector.Pipeline.Interfaces
{
    public interface IPipeFactory<T> : IPipeProvider<T>
    {
        BatchingPipe<T> Create(); 
    }

    public interface IPipeProvider<T>
    {
        event PipeFactoryDelegates.Created<T> PipeCreatedEvent;
    }

    public static class PipeFactoryDelegates
    {
        public delegate void Created<T>(IPipeFactory<T> sender, BatchingPipe<T> pipe);
    }
}