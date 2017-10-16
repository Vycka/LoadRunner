namespace Viki.LoadRunner.Engine.Executor.Strategy.Workers.Interfaces
{
    public interface IPrewait
    {
        void Wait(ref bool stop);
    }
}