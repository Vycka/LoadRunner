namespace Viki.LoadRunner.Engine.Core.Worker.Interfaces
{
    public interface IWork
    {
        void Init();
        void Execute();
        void Cleanup();

        void Stop();
    }
}