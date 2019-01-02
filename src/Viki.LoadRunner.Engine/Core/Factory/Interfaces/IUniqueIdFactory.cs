namespace Viki.LoadRunner.Engine.Core.Factory.Interfaces
{
    public interface IUniqueIdFactory<T>
    {
        T Next();

        T Current { get; }
    }
}