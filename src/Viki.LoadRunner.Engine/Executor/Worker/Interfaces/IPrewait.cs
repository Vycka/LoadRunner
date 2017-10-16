namespace Viki.LoadRunner.Engine.Executor.Worker.Interfaces
{
    public interface IPrewait
    {
        void Wait(ref bool stop);
    }
}