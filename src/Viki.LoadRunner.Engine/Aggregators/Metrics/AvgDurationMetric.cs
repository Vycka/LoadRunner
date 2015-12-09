using System;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class AvgDurationMetric : IMetric
    {
        private readonly FlexiGrid<string, AverageTimeCalculator> _grid = new FlexiGrid<string, AverageTimeCalculator>(() => new AverageTimeCalculator());
        private readonly string[] _ignoredCheckpoints;

        private static readonly Checkpoint BlankCheckpoint = new Checkpoint("", TimeSpan.Zero);

        public AvgDurationMetric(params string[] ignoredCheckpoints)
        {
            if (ignoredCheckpoints == null)
                throw new ArgumentNullException(nameof(ignoredCheckpoints));

            _ignoredCheckpoints = ignoredCheckpoints;
        }

        IMetric IMetric.CreateNew()
        {
            return new AvgDurationMetric(_ignoredCheckpoints);
        }

        void IMetric.Add(IResult result)
        {
            ICheckpoint previousCheckpoint = BlankCheckpoint;
            foreach (ICheckpoint checkpoint in result.Checkpoints)
            {
                if (_ignoredCheckpoints.All(name => name != checkpoint.Name))
                {
                    string key = "Avg: " + checkpoint.Name;
                    TimeSpan momentDiff = TimeSpan.FromTicks(checkpoint.TimePoint.Ticks - previousCheckpoint.TimePoint.Ticks);

                    _grid[key].AddSample(momentDiff);
                }
                previousCheckpoint = checkpoint;
            }
        }

        string[] IMetric.ColumnNames => _grid.Keys.ToArray();
        object[] IMetric.Values => _grid.Values.Select(v => (object)Convert.ToInt64(v.GetAverage().TotalMilliseconds)).ToArray();
    }

    public class AverageTimeCalculator
    {
        private int _sampleCount = 0;
        private TimeSpan _timeSum = TimeSpan.Zero;

        public void AddSample(TimeSpan timeSpan)
        {
            _sampleCount++;
            _timeSum += timeSpan;
        }

        public TimeSpan GetAverage()
        {
            return TimeSpan.FromTicks(_timeSum.Ticks /_sampleCount);
        }
    }
}