using System;

namespace Viki.LoadRunner.Engine.Executor.Scenario.Interfaces
{
    public interface IIterationContextControl : IIterationContext, IIterationResult
    {
        void Start();
        void Stop();
        void Reset(int threadIterationId, int globalIterationId);
        void SetError(Exception error);
    }
}