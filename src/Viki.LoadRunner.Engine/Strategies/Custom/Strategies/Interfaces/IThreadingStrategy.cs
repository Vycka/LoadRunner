using Viki.LoadRunner.Engine.Core.Pool.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces
{
    public interface IThreadingStrategy
    {
        void Setup(IThreadPool pool);

        void HeartBeat(IThreadPool pool, ITestState state);
    }
}