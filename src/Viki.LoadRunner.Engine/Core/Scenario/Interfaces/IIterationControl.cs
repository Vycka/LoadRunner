using System;

namespace Viki.LoadRunner.Engine.Core.Scenario.Interfaces
{
    public interface IIterationControl : IIteration, IIterationResult
    {
        void Start();
        void Stop();
        void Reset(int threadIterationId, int globalIterationId);
    }
}