namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public interface IUniqueIdFactory<T>
    {
        T Next();
    }
}