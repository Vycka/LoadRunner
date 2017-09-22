namespace Viki.LoadRunner.Engine.Executor.Threads.Interfaces
{
    public interface ICounter
    {
        int Value { get; }

        int Add(int count);
    }
}