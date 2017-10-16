namespace Viki.LoadRunner.Engine.Executor.Factory.Interfaces
{
    public interface IUniqueIdFactory<T>
    {
        T Next();

        T Current { get; }
    }
}