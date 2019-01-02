namespace Viki.LoadRunner.Engine.Core.Scenario.Interfaces
{
    public interface IGlobalCounters
    {
        int LastGlobalIterationId { get; }
        int ErrorCount { get; }
    }
}