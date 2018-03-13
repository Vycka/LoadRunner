using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Viki.LoadRunner.Engine.Strategies.Replay.Data.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Data.Readers
{
    public class ReplayDataReader : IReplayDataReader
    {
        private readonly DataItem[] _data;
        private int _readIndex;

        public ReplayDataReader(ICollection<DataItem> data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _data = data.ToArray();
        }

        public void Begin()
        {
            _readIndex = -1;
        }

        public DataItem Next(int threadId)
        {
            int current = Interlocked.Increment(ref _readIndex);

            DataItem result = null;
            if (current < _data.Length)
                result = _data[current];

            return result;
        }

        public void End()
        {
        }
    }
}