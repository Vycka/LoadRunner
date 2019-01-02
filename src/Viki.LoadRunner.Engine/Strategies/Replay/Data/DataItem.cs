using System;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Data
{
    public class DataItem
    {
        public TimeSpan TimeStamp;

        public object Value;

        public DataItem(TimeSpan timeStamp, object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            TimeStamp = timeStamp;
            Value = value;
        }
    }
}