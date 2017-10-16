namespace Viki.LoadRunner.Engine.Executor.Counter.Interfaces
{
    public interface ICounter
    {
        int Value { get; }

        int Add(int count);
    }
}