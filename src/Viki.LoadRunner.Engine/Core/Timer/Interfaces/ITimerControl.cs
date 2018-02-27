namespace Viki.LoadRunner.Engine.Core.Timer.Interfaces
{
    public interface ITimerControl : ITimer
    {
        void Start();
        void Stop();
    }
}