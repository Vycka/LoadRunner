using System;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scenario.Interfaces
{
    public interface IIterationControl : IIteration, IIterationResult
    {
        void Start();
        void Stop();
        void Reset(int threadIterationId, int globalIterationId);
        void SetError(Exception error);
    }
}