using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
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

        public void Begin(ITestState testState)
        {
            Array.ForEach(_readers, r => r.Begin(testState));
        }

        public DataItem Next(int threadId, ref bool stop)
        {
            return _readers[threadId % _readers.Length].Next(threadId, ref stop);
        }

        public void End()
        {
            Array.ForEach(_readers, r => r.End());
        }
    }
}