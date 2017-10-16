namespace Viki.LoadRunner.Engine.Executor.Strategy.Timer.Interfaces
{
    public interface ITimerControl : ITimer
    {
        void Start();
        void Stop();
    }
}