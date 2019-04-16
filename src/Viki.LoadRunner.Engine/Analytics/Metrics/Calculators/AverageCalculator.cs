namespace Viki.LoadRunner.Engine.Analytics.Metrics.Calculators
{
    public class AverageCalculator
    {
        private int _sampleCount = 0;
        private double _sum = 0;

        public void Add(double duration)
        {
            _sampleCount++;
            _sum += duration;
        }

        public double GetAverage()
        {
            return _sum / _sampleCount;
        }
    }
}