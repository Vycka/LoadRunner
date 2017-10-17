namespace Viki.LoadRunner.Engine.Strategies.Interfaces
{
    public interface IStrategy
    {
        void Start();

        bool HeartBeat();

        void Stop();
    }
}