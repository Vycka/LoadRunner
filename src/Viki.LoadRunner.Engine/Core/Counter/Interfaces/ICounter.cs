namespace Viki.LoadRunner.Engine.Core.Counter.Interfaces
{
    public interface ICounter
    {
        int Value { get; }

        int Add(int count);
    }
}