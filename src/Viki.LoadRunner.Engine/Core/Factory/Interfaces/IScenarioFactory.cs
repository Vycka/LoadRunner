
namespace Viki.LoadRunner.Engine.Core.Factory.Interfaces
{
    //public interface IScenarioFactory<T> : IScenarioFactory
    //{
    //    T Create();
    //}

    public interface IScenarioFactory
    {
        object Create();
    }
}