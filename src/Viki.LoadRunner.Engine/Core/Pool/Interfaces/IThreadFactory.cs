using Viki.LoadRunner.Engine.Core.Worker.Interfaces;
namespace Viki.LoadRunner.Engine.Core.Pool.Interfaces
{
    public interface IThreadFactory
    {
        IThread Create();
    }
}