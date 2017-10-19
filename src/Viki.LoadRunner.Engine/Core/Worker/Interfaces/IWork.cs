namespace Viki.LoadRunner.Engine.Core.Worker.Interfaces
{
    public interface IWork
    {
        void Init();
        void Execute(ref bool stop);
        void Cleanup();
    }
}