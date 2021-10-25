using Viki.LoadRunner.Engine.Core.State.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Interfaces
{
    public interface IStrategy
    {
        ITestState Start();

        bool HeartBeat();

        void Stop();
    }
}