namespace Viki.LoadRunner.Engine.Executor.Timer
{
    public interface ITimerControl : ITimer
    {
        void Start();
        void Stop();
    }
}