namespace Viki.LoadRunner.Engine.Analytics.Metrics.Calculators
{
    public class AverageCalculator
    {
        public int SampleCount { get; private set; } = 0;
        public double Sum { get; private set; } = 0;

        public void Add(double duration)
        {
            SampleCount++;
            Sum += duration;
        }

        public double GetAverage()
        {
            return Sum / SampleCount;
        }
    }
}