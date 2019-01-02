namespace Viki.LoadRunner.Engine.Core.Factory.Interfaces
{
    public interface IFactory<out T>
    {
        T Create(int threadId);
    }
}