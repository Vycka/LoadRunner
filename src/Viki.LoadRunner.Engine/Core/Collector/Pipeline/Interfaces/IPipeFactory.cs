namespace Viki.LoadRunner.Engine.Core.Collector.Pipeline.Interfaces
{
    public interface IPipeFactory<T>
    {
        BatchingPipe<T> Create();
    }
}