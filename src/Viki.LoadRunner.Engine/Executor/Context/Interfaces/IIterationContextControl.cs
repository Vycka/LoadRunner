using System;

namespace Viki.LoadRunner.Engine.Executor.Context.Interfaces
{
    public interface IIterationContextControl : IIterationContext, IIterationResult
    {
        void Start();
        void Stop();
        void Reset(int threadIterationId, int globalIterationId);
        void SetError(Exception error);
    }
}