using System;
using System.Linq;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class TransactionsPerSecMetric : IMetric
    {
        private uint _count = 0;
        private TimeSpan _iterationStartedMin = TimeSpan.MaxValue;
        private TimeSpan _iterationFinishedMax = TimeSpan.MinValue; 

        IMetric IMetric.CreateNew()
        {
            return new TransactionsPerSecMetric();
        }

        void IMetric.Add(IResult result)
        {
            if (result.Checkpoints.All(c => c.Error == null))
            {
                if (result.IterationStarted < _iterationStartedMin)
                    _iterationStartedMin = result.IterationStarted;

                if (result.IterationFinished > _iterationFinishedMax)
                    _iterationFinishedMax = result.IterationFinished;

                _count++;
            }
        }

        private int GetSecondsPassed()
        {
            return (int)(_iterationFinishedMax = _iterationStartedMin).TotalSeconds;
        }

        string[] IMetric.ColumnNames { get; } = {"TPS"};
        object[] IMetric.Values => _count == 0 ? new object[] {0.0} : new object[] { _count/GetSecondsPassed() };
    }
}