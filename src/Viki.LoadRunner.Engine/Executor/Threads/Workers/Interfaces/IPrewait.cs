namespace Viki.LoadRunner.Engine.Executor.Threads.Workers.Interfaces
{
    public interface IPrewait
    {
        void Wait(ref bool stop);
    }
}