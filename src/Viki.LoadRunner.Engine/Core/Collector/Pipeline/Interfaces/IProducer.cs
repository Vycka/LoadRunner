namespace Viki.LoadRunner.Engine.Core.Collector.Pipeline.Interfaces
{
    public interface IProducer<T>
    {
        void Produce(T item);

        void ProducingCompleted();
    }
}