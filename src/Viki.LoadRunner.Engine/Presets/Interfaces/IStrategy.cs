using Viki.LoadRunner.Engine.Executor.Strategy.Counters.Interfaces;

namespace Viki.LoadRunner.Engine.Presets.Interfaces
{
    public interface IStrategy
    {
        void Start();

        bool HeartBeat();

        void Stop();
    }
}