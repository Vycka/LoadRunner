namespace Viki.LoadRunner.Engine.Core.Generator.Interfaces
{
    public interface IUniqueIdGenerator<out T>
    {
        T Next();

        T Current { get; }
    }
}