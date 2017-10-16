namespace Viki.LoadRunner.Engine.Executor.Strategy.Factory.Interfaces
{
    public interface IUniqueIdFactory<T>
    {
        T Next();

        T Current { get; }
    }
}