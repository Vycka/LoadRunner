using System;

namespace Viki.LoadRunner.Engine.Executor.Scenario.Interfaces
{
    public interface IIterationControl : IIteration, IIterationResult
    {
        void Start();
        void Stop();
        void Reset(int threadIterationId, int globalIterationId);
        void SetError(Exception error);
    }
}