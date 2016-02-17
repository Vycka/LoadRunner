using System;
using System.Linq;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class TransactionsPerSecMetric : IMetric
    {
        private TimeSpan _iterationStartedMin;
        private TimeSpan _iterationFinishedMax;
        
        private uint _count;

        public TransactionsPerSecMetric()
        {
            _count = 0;

            _iterationStartedMin = TimeSpan.MaxValue;
            _iterationFinishedMax = TimeSpan.MinValue;
        }

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
            if (_count == 0)
                return 0;

            return (int)(_iterationFinishedMax - _iterationStartedMin).TotalSeconds;
        }

        string[] IMetric.ColumnNames { get; } = {"TPS"};

        object[] IMetric.Values
        {
            get
            {
                int passedSeconds = GetSecondsPassed();

                if (passedSeconds == 0)
                    return new object[] {0.0};
                else
                    return new object[] {((double)_count)/GetSecondsPassed()};
            }
        }
    }
}