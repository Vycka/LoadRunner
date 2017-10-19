namespace Viki.LoadRunner.Engine.Core.Worker.Interfaces
{
    public interface IErrorHandler
    {
        void Assert();

        void Register(IThread thread);
    }
}