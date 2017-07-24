namespace Viki.LoadRunner.Engine.Executor.Threads.Interfaces
{
    public interface IUniqueIdFactory<T>
    {
        T Next();

        T Current { get; }
    }
}