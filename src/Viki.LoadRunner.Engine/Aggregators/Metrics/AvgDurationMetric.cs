using System;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class AvgDurationMetric : IMetric
    {
        readonly FlexiGrid<string, AverageTimeCalculator> _grid = new FlexiGrid<string, AverageTimeCalculator>(() => new AverageTimeCalculator());

        public IMetric CreateNew()
        {
            return new AvgDurationMetric();
        }

        public void Add(TestContextResult result)
        {
            foreach (Checkpoint checkpoint in result.Checkpoints)
            {
                string key = checkpoint.CheckpointName + " Avg";

                _grid[key].AddSample(checkpoint.TimePoint);
            }

        }

        public string[] ColumnNames => _grid.Select(i => i.Key).ToArray();
        public object[] Values => _grid.Select(i => (object)i.Value.GetAverage()).ToArray();
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