using System;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

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

        IMetric<IResult> IMetric<IResult>.CreateNew()
        {
            return new TransactionsPerSecMetric();
        }

        void IMetric<IResult>.Add(IResult result)
        {
            if (result.IterationStarted < _iterationStartedMin)
                _iterationStartedMin = result.IterationStarted;

            if (result.IterationFinished > _iterationFinishedMax)
                _iterationFinishedMax = result.IterationFinished;

            if (result.Checkpoints.All(c => c.Error == null))
            {
                _count++;
            }
        }

        private TimeSpan GetDurationDelta()
        {
            if (_count == 0)
                return TimeSpan.Zero;

            return _iterationFinishedMax - _iterationStartedMin;
        }

        string[] IMetric<IResult>.ColumnNames { get; } = { "TPS" };

        object[] IMetric<IResult>.Values
        {
            get
            {
                TimeSpan delta = GetDurationDelta();

                if (delta == TimeSpan.Zero)
                    return new object[] {0.0};
                else
                    return new object[] {(double)_count / delta.TotalSeconds };
            }
        }
    }
}