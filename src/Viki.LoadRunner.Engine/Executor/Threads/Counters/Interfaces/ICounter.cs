namespace Viki.LoadRunner.Engine.Executor.Threads.Counters.Interfaces
{
    public interface ICounter
    {
        int Value { get; }

        int Add(int count);
    }
}