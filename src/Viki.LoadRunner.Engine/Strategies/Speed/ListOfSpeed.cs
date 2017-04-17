using System;
using System.Linq;

namespace Viki.LoadRunner.Engine.Strategies.Speed
{
    public class ListOfSpeed : ISpeedStrategy
    {
        private readonly TimeSpan _period;
        private readonly double[] _iterationPerSecValues;

        private readonly TimeSpan _maxSpeed;
        protected static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        public ListOfSpeed(TimeSpan period, params double[] iterationPerSecValues)
        {
            if (iterationPerSecValues == null)
                throw new ArgumentNullException(nameof(iterationPerSecValues));
            if (iterationPerSecValues.Length == 0)
                throw new ArgumentException("At least one iteration speed must be provided", nameof(iterationPerSecValues));

            _period = period;
            _iterationPerSecValues = iterationPerSecValues;
            _maxSpeed = TimeSpan.FromTicks((long)(OneSecond.Ticks /  _iterationPerSecValues.Last()));
        }

        public virtual TimeSpan GetDelayBetweenIterations(TimeSpan testExecutionTime)
        {
            long index = testExecutionTime.Ticks / _period.Ticks;

            if (index < _iterationPerSecValues.Length)
                return TimeSpan.FromTicks((long) (OneSecond.Ticks / _iterationPerSecValues[index])); // TODO: This math can be cached in c-tor

            return _maxSpeed;
        }
    }
}