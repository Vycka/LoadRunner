namespace Viki.LoadRunner.Engine.Executor.Threads.Factory.Interfaces
{
    public interface IUniqueIdFactory<T>
    {
        T Next();

        T Current { get; }
    }
}