namespace Viki.LoadRunner.Engine.Executor.Strategy.Counters.Interfaces
{
    public interface ICounter
    {
        int Value { get; }

        int Add(int count);
    }
}