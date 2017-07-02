using System;
using System.Linq;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Speed
{
    /// <summary>
    /// Define manually a table of various speed values which will be used one by one every provided time period
    /// </summary>
    public class ListOfSpeed : FixedSpeed, ISpeedStrategy
    {
        private readonly TimeSpan _period;
        private readonly double[] _iterationPerSecValues;

        private readonly TimeSpan _maxSpeed;
        protected static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Define manually a table of various speed values which will be used one by one every provided time period
        /// </summary>
        /// <param name="period">time period after which value from the next index will be used</param>
        /// <param name="iterationPerSecValues">Speed values for each time period.
        /// Once all values are consumed, speed will stay at value from last index</param>
        public ListOfSpeed(TimeSpan period, params double[] iterationPerSecValues)
        {
            if (iterationPerSecValues == null)
                throw new ArgumentNullException(nameof(iterationPerSecValues));
            if (iterationPerSecValues.Length == 0)
                throw new ArgumentException("At least one iteration speed must be provided", nameof(iterationPerSecValues));

            _period = period;
            _iterationPerSecValues = iterationPerSecValues;
            _maxSpeed = TimeSpan.FromTicks((long)(OneSecond.Ticks /  _iterationPerSecValues.Last()));

            SetSpeed(iterationPerSecValues[0]);
        }

        public virtual TimeSpan GetDelayBetweenIterations(TimeSpan testExecutionTime)
        {
            long index = testExecutionTime.Ticks / _period.Ticks;

            if (index < _iterationPerSecValues.Length)
                return TimeSpan.FromTicks((long) (OneSecond.Ticks / _iterationPerSecValues[index]) + 1) ; // TODO: This math can be cached in c-tor

            return _maxSpeed;
        }

        protected int GetIndex(CoordinatorContext context)
        {
            int index = (int)(context.Timer.Value.Ticks / _period.Ticks);

            if (index >= _iterationPerSecValues.Length)
                index = _iterationPerSecValues.Length - 1;

            return index;
        }

        public new void Adjust(CoordinatorContext context)
        {
            SetSpeed(_iterationPerSecValues[GetIndex(context)]);

            base.Adjust(context);
        }
    }
}