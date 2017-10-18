using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Viki.LoadRunner.Engine.Strategies.Replay.Reader.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Reader
{
    public class EnumerableDataReader<T> : IDataReader<T>
    {
        private readonly LogItem<T>[] _data;
        private int _readIndex;

        public EnumerableDataReader(ICollection<LogItem<T>> data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _data = data.ToArray();
        }

        public void Start()
        {
            _readIndex = -1;
        }

        public LogItem<T> Next()
        {
            int current = Interlocked.Increment(ref _readIndex);

            return _data[current];
        }

        public void End()
        {
        }
    }
}