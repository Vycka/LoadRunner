using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Strategies.Replay.Data.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Data.Readers
{
    public class PartitionedDataReader : IReplayDataReader
    {
        private readonly IReplayDataReader[] _readers;

        public PartitionedDataReader(ICollection<IReplayDataReader> readers)
        {
            _readers = readers.ToArray();
        }

        public void Begin()
        {
            Array.ForEach(_readers, r => r.Begin());
        }

        public DataItem Next(int threadId)
        {
            return _readers[threadId % _readers.Length].Next(threadId);
        }

        public void End()
        {
            Array.ForEach(_readers, r => r.End());
        }
    }
}