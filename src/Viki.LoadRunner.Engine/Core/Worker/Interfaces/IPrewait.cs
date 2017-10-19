namespace Viki.LoadRunner.Engine.Core.Worker.Interfaces
{
    public interface IPrewait
    {
        void Wait(ref bool stop);
    }
}