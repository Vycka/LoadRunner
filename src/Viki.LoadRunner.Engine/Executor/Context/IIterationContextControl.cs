using System;

namespace Viki.LoadRunner.Engine.Executor.Context
{
    public interface IIterationContextControl : IIterationContext, IIterationResult
    {
        void Start();
        void Stop();
        void Reset(int threadIterationId, int globalIterationId);
        void SetError(Exception error);
    }
}